using Homify.Models;
using Homify.Models.dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;


namespace Homify.Controllers
{
    [RoutePrefix("api/rentals")]
    public class RentalsController : ApiController
    {

        private HomifyDBEntities2 _homifyDB = new HomifyDBEntities2();

        //register a rental
        [HttpPost]
        [Route("")]
        public IHttpActionResult addRental([FromBody] Rental rental)
        {
            if(rental != null)
            {
                _homifyDB.Rentals.Add(rental);
                _homifyDB.SaveChanges();
                return Ok(new { message = "Client Added successfully" });
            }
            return BadRequest("invalid client data");
        }

        //get rentals that belong to a user
        [HttpGet]
        [Route("getRentals/{id:int}")]
        public IHttpActionResult getRentals(int id)
        {
            var trackUserRentals = _homifyDB.Rentals.Where(x => x.userId == id).ToList();
            if(trackUserRentals.Any())
            {
                var rental = trackUserRentals.Select(r => new Rentaldto
                {
                    clientName = r.clientName,
                    clientContact = r.clientContact,
                    clientEmail = r.clientEmail,
                    clientLocation = r.clientLocation,
                    rent = r.rent,
                    billingPeriod = r.billingPeriod,
                    due = r.due,
                }).ToList();
                return Ok(rental);
            }
            else if(trackUserRentals.Count <= 0)
            {
                return Ok(new { message = "no rentals yet" });
            }
            else
            {
                return BadRequest();
            }
        }

        //api to update rentals
        [HttpPut]
        [Route("updateRentals/{userId:int}/{rentalId:int}")]
        public IHttpActionResult updateRental(int userId, int rentalId, [FromBody] Rental rentalUpdate)
        {
            var trackRental = _homifyDB.Rentals.Where(x => x.rentalId == rentalId && x.userId == userId).FirstOrDefault();

            if(trackRental != null)
            {
                if(rentalUpdate != null)
                {
                    trackRental.clientName = rentalUpdate.clientName;
                    trackRental.clientEmail = rentalUpdate.clientEmail;
                    trackRental.clientContact = rentalUpdate.clientContact;
                    trackRental.clientLocation = rentalUpdate.clientLocation;
                    trackRental.rent = rentalUpdate.rent;
                    trackRental.billingPeriod = rentalUpdate.billingPeriod;
                    trackRental.due = rentalUpdate.due;
                    _homifyDB.SaveChanges();
                    return Ok(new { message = $"{trackRental.clientName} data has been updated successfully" });
                }
                else
                {
                    return BadRequest("Invalid Rental Data");
                }
            }
            return NotFound();
        }


        //api to delete a rental
        [HttpDelete]
        [Route("deleteRentals/{userId:int}/{rentalId:int}")]
        public IHttpActionResult deleteRentals(int userId, int rentalId)
        {
            var trackRental = _homifyDB.Rentals.Where(x => x.rentalId == rentalId && x.userId == userId).FirstOrDefault();
            if (trackRental != null)
            {
                _homifyDB.Rentals.Remove(trackRental);
                _homifyDB.SaveChanges();
                return Ok(new { message = "Rental deleted" });
            }
            return NotFound();
        }

    }
}