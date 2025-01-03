using Homify.Models;
using Homify.Models.dto;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Hosting;
using System.Web.Http;
using System.Web.Http.Results;
using System.Web.UI.WebControls;

namespace Homify.Controllers
{
    [RoutePrefix("api/properties")] // adding a prefix to every api in this controller
    public class PropertiesController : ApiController
    {
        private HomifyDBEntities2 _homifyDB = new HomifyDBEntities2();
        private List<PropertyImage> newImages;

        //api to return all available prperties
        [HttpGet]
        [Route("")]
        public IHttpActionResult getProperty()
        {
            var properties = _homifyDB.Properties.Include("Descriptions").Include("PropertyImages").Select(p => new Propertydto
            {
                propertyId = p.propertyId,
                userId = p.userId,
                price = p.price,
                location = p.location,
                status = p.status,
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
                PropertyImages = p.PropertyImages.Select(i => new PropertyImagesdto
                {
                    imageId = i.imageId,
                    propertyId = i.propertyId,
                    imagePath = i.imagePath,
                }).ToList()
            }).ToList();

            return Json(properties);
        }

        //api to return propeties with the given id
        [HttpGet]
        [Route("{id:int}")]
        public IHttpActionResult getPropertyById(int id)
        {
            var property = _homifyDB.Properties.Where(x => x.propertyId == id).Select(p => new Propertydto
            {
                propertyId = p.propertyId,
                userId = p.userId,
                price = p.price,
                location = p.location,
                status = p.status,
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
                PropertyImages = p.PropertyImages.Select(i => new PropertyImagesdto
                {
                    imageId = i.imageId,
                    propertyId = i.propertyId,
                    imagePath = i.imagePath,
                }).ToList()
            }).FirstOrDefault();

            if(property != null)
            {
                return Json(property);
            }
            return NotFound();
        }



        //api for creating properties
        [HttpPost]
        [Route("")]
        public async Task<IHttpActionResult> createProperty()
        {
            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            // Get JSON property data
            HttpContent propertyDataContent = provider.Contents.FirstOrDefault(c => c.Headers.ContentDisposition.Name.Trim('"') == "property");
            if (propertyDataContent == null)
            {
                return BadRequest("Property data is missing");
            }

            var propertyJson = await propertyDataContent.ReadAsStringAsync();
            var property = JsonConvert.DeserializeObject<Property>(propertyJson);

            if (property == null)
            {
                return BadRequest("Invalid property data");
            }

            if (property == null)
            {
                return BadRequest("property data required");
            }

            _homifyDB.Properties.Add(property);

            //loop to add each description one by one
            foreach(var description in property.Descriptions)
            {
                description.propertyId = property.propertyId;
                _homifyDB.Descriptions.Add(description);
            }

            // Handle image files
            var imageFiles = provider.Contents.Where(c => c.Headers.ContentDisposition.Name.Trim('"') == "imageFiles").ToList();

            if (imageFiles != null && imageFiles.Any())
            {
                var uploadPath = HttpContext.Current.Server.MapPath("~/Content/uploads");
                Directory.CreateDirectory(uploadPath); // Ensure directory exists

                for (int i = 0; i < imageFiles.Count; i++)
                {
                    var fileBytes = await imageFiles[i].ReadAsByteArrayAsync();
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFiles[i].Headers.ContentDisposition.FileName.Trim('"'));
                    var filePath = Path.Combine(uploadPath, fileName);

                    using (var filestream = new FileStream(filePath,FileMode.Create, FileAccess.Write))
                    {
                        filestream.Write(fileBytes, 0, fileBytes.Length);
                    }

                    var imageUrl = $"/Content/uploads/{fileName}";

                    _homifyDB.PropertyImages.Add(new PropertyImage
                    {
                        propertyId = property.propertyId,
                        imagePath = imageUrl
                    });
                }
            }

            await _homifyDB.SaveChangesAsync();

            return  Ok(new { message = "Property created successfully", propertyId = property.propertyId });
        }

        //api for updating properties
        [HttpPut]
        [Route("{id:int}")]
        public async Task<IHttpActionResult> updateProperty(int id)
        {
            //looking for the existing property to update
            var existingProperty = _homifyDB.Properties.Include("Descriptions").Include("PropertyImages").FirstOrDefault(p => p.propertyId == id);

            if (!Request.Content.IsMimeMultipartContent())
            {
                throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
            }

            var provider = new MultipartMemoryStreamProvider();
            await Request.Content.ReadAsMultipartAsync(provider);

            // Get JSON property data
            HttpContent propertyDataContent = provider.Contents.FirstOrDefault(c => c.Headers.ContentDisposition.Name.Trim('"') == "property");
            if (propertyDataContent == null)
            {
                return BadRequest("Property data is missing");
            }

            var propertyJson = await propertyDataContent.ReadAsStringAsync();
            var updateProperty = JsonConvert.DeserializeObject<Property>(propertyJson);


            if (existingProperty == null)
            {
                return NotFound();
            }

            existingProperty.price = updateProperty.price;
            existingProperty.location = updateProperty.location;
            existingProperty.status = updateProperty.status;

            //updating description on description
            foreach(var updateDescription in updateProperty.Descriptions)
            {
                var existingDescriptions = existingProperty.Descriptions.FirstOrDefault(d => d.description1 == updateDescription.description1);
                if(existingDescriptions != null)
                {
                    existingDescriptions.landType = updateDescription.landType;
                    existingDescriptions.size = updateDescription.size;
                    existingDescriptions.houseType = updateDescription.houseType;
                    existingDescriptions.bedRooms = updateDescription.bedRooms;
                    existingDescriptions.bathRooms = updateDescription.bathRooms;
                    existingDescriptions.parking = updateDescription.parking;
                    existingDescriptions.YearBuilt = updateDescription.YearBuilt;
                    existingDescriptions.Amentities = updateDescription.Amentities;
                }
                else
                {
                    updateDescription.propertyId = id;
                    _homifyDB.Descriptions.Add(updateDescription);
                }
            }

            //remove old descriptions 
            var descriptionToRemove = existingProperty.Descriptions.Where(d => !updateProperty.Descriptions.Any(u => u.description1 == d.description1)).ToList();
            foreach(var description in descriptionToRemove)
            {
                _homifyDB.Descriptions.Remove(description);
            }

            //restore images for the property
            var imageFiles = provider.Contents.Where(c => c.Headers.ContentDisposition.Name.Trim('"') == "imageFiles").ToList();

            if (imageFiles != null && imageFiles.Any())
            {
                var uploadPath = HttpContext.Current.Server.MapPath("~/Content/uploads");
                Directory.CreateDirectory(uploadPath); // Ensure directory exists

                for (int i = 0; i < imageFiles.Count; i++)
                {
                    var fileBytes = await imageFiles[i].ReadAsByteArrayAsync();
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFiles[i].Headers.ContentDisposition.FileName.Trim('"'));
                    var filePath = Path.Combine(uploadPath, fileName);
             

                    using (var filestream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        filestream.Write(fileBytes, 0, fileBytes.Length);
                    }

                    var imageUrl = $"/Content/uploads/{fileName}";

                    var image =  new PropertyImage
                    {
                        propertyId = id,
                        imagePath = imageUrl
                    };
                    _homifyDB.PropertyImages.Add(image);
                    updateProperty.PropertyImages.Add(image);
                }
            }

            //remove old image urls that arenot in the updated urls
            var imagestoRemove = existingProperty.PropertyImages.Where(img => !updateProperty.PropertyImages.Any(u => u.imageId == img.imageId)).ToList();
            foreach(var image in imagestoRemove)
            {
                _homifyDB.PropertyImages.Remove(image);
            }

            await _homifyDB.SaveChangesAsync();
            return Ok(new { message="Property updated successfully", propertyId =id }); 
        }

        //api to delete a property
        [HttpDelete]
        [Route("{id:int}")]
        public IHttpActionResult deleteProperty(int id)
        {
            var property = _homifyDB.Properties.Include("Descriptions").Include("PropertyImages").Where(x => x.propertyId == id).FirstOrDefault();
            if(property != null)
            {
                //this is used to delete the property togther with its related data
                _homifyDB.Descriptions.RemoveRange(property.Descriptions);
                _homifyDB.PropertyImages.RemoveRange(property.PropertyImages);
                _homifyDB.Properties.Remove(property);
                _homifyDB.SaveChanges();
                return Ok(new { message = "Property deleted successfully" });
            }
            return NotFound();
        }





    }
}
