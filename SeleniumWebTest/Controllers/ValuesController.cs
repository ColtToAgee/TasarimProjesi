using SeleniumEntity.Models;
using SeleniumEntity.ViewModels;
using SeleniumService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Cors;

namespace SeleniumWebTest.Controllers
{
    [EnableCors(origins:"*",headers:"*",methods:"*")]
    public class ValuesController : ApiController
    {
        // GET api/values
        public List<DoctorProfilesViewModel> Get()
        {
            var userList = new List<DoctorProfilesViewModel>();
            using (var db = new DbService())
            {
                userList = db.Query<DoctorProfilesViewModel>(@"
                            Select
                            dp.Id,
                            dp.DoctorName,
                            dp.DoctorEmail,
                            dp.DoctorHospital,
                            dp.DoctorImageLink,
                            dp.DoctorPoliclinic,
                            pc.PoliclinicName as [DoctorPoliclinicName],
                            dp.DoctorTitle,
                            tl.TitleName as [DoctorTitleName],
                            dp.RowStateId
                            from DoctorProfiles as dp
                            Inner Join Policlinics as pc 
                            on pc.Id=dp.DoctorPoliclinic
                            Inner Join Titles as tl
                            on tl.Id = dp.DoctorTitle

                            ");
            }
            return userList;
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
