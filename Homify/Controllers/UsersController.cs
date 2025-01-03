using Homify.Models;
using Homify.Models.dto;
using Homify.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
//using System.Web.Mvc;

namespace Homify.Controllers
{
    [RoutePrefix("api/users")]
    public class UsersController : ApiController
    {
        private HomifyDBEntities2 _homifyDB = new HomifyDBEntities2();

        //api to get all users
        [HttpGet]
        [Route("")]
        public IHttpActionResult getUsers()
        {
            var users = _homifyDB.Users.Select(u => new Usersdto
            {
                userId = u.userId,
                userName = u.userName,
                firstName = u.firstName,
                lastName = u.lastName,
                passWord = u.passWord,
                emailContact = u.emailContact,
            }).ToList();

            return Json(users);
        }

        //api to register a user
        [HttpPost]
        [Route("register")]
        public IHttpActionResult registerUser([FromBody] User user)
        {
            if (user != null)
            {
                _homifyDB.Users.Add(user);
                _homifyDB.SaveChanges();

                //send an email to the user after siging up
                var emailService = new EmailServices();
                emailService.SendEmail(user.emailContact, "Welcome to Homify!", $"Dear {user.userName}, you have been successfully registered with Homify.");

                return Ok(new { message = "User registered successfully" });
            }
            return BadRequest("Invalid property data.");
        }

        //api to update user data
        [HttpPut]
        [Route("update/{id:int}")]
        public IHttpActionResult updateUser(int id, [FromBody] User userUpdate)
        {
            var trackUser = _homifyDB.Users.Where(x => x.userId == id).FirstOrDefault();    
            if(trackUser != null)
            {
                trackUser.userName = userUpdate.userName;
                trackUser.firstName = userUpdate.firstName; 
                trackUser.lastName = userUpdate.lastName;
                trackUser.emailContact = userUpdate.emailContact;
                _homifyDB.SaveChanges();
                return Ok(new { message = $"{trackUser.userName} data has been updated successfully" });
            }
            return BadRequest("User not found");
        }

        //api to delete a user 
        [HttpDelete]
        [Route("delete/{id:int}")]
        public IHttpActionResult deleteUser(int id)
        {
            var trackUser = _homifyDB.Users.Where(x => x.userId == id).FirstOrDefault();
            if(trackUser != null) 
            {
                //track properties that belong to the user being deleted
                var trackUserProperties = _homifyDB.Properties.Include("Descriptions").Include("PropertyImages").Where(p => p.userId == trackUser.userId).ToList();
                if(trackUserProperties.Count > 0)
                {
                    foreach(var property in trackUserProperties)
                    {
                        //this is used to delete the property togther with its related data
                        _homifyDB.Descriptions.RemoveRange(property.Descriptions);
                        _homifyDB.PropertyImages.RemoveRange(property.PropertyImages);
                        _homifyDB.Properties.Remove(property);
                    }
                }
                //track rentals too
                var trackRentals = _homifyDB.Rentals.Where(x =>  x.userId == id).ToList();
                if(trackRentals.Count > 0)
                {
                    foreach(var rental in trackRentals)
                    {
                        _homifyDB.Rentals.Remove(rental);
                    }
                }
                //delete the user also
                _homifyDB.Users.Remove(trackUser);
                _homifyDB.SaveChanges();
                return Ok(new { message = "User deleted successfully" });
            }
            return NotFound();            
            
        }




        //api to login users and return user with related properties
        [HttpPost]
        [Route("login")]
        public IHttpActionResult loginUser([FromBody] User user)
        {
            //check if the sent data to the api is not empty
            if (user != null)
            {
                //track if the user exists in the database
                var trackUser = _homifyDB.Users.Where(x => x.userName == user.userName && x.passWord == user.passWord).FirstOrDefault();
                //check if the user exists in the database
                if (trackUser != null)
                {
                    //track properties owned by the logged in user
                    var trackuserProperties = _homifyDB.Properties.Include("Descriptions").Include("PropertyImages").Where(p => p.userId == trackUser.userId).ToList();
                    //check whether the user has any properties
                    if (trackuserProperties.Any())
                    {
                        var property = trackuserProperties.Select(p => new Propertydto
                        {
                            propertyId = p.propertyId,
                            userId = p.userId,
                            price = p.price,
                            location = p.location,
                            status = p.status,
                            //for fetching list of decriptions for the property
                            Descriptions = p.Descriptions.Select(d => new Descriptionsdto
                            {
                                description1 = d.description1,
                                propertyId = d.propertyId,
                                landType = d.landType,
                                size = d.size,
                                houseType = d.houseType,
                                bedRooms = d.bedRooms,
                                parking = d.parking,
                                bathRooms = d.bedRooms,
                                YearBuilt = d.YearBuilt,
                                Amentities = d.Amentities,
                            }).ToList(),
                            //for fetching images of the property
                            PropertyImages = p.PropertyImages.Select(i => new PropertyImagesdto
                            {
                                imageId = i.imageId,
                                propertyId = i.propertyId,
                                imagePath = i.imagePath,
                            }).ToList()
                        }).ToList();
                        return Ok(property);
                    }
                    // if the user has no properties
                    else
                    {
                        return Ok(new { message = "no properties yet" });
                    }
                }
                //if the user dont exist in the database
                else
                {
                    return BadRequest("Invalid user data.");
                }
            }
            return BadRequest("Invalid user data.");
        }

    }
}