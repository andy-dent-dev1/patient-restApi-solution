using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.WebApi;
using NUnit.Framework;
using RestApi.Controllers;
using RestApi.Interfaces;
using RestApi.Models;
using RestApi.Test.App_Start;

namespace RestApi.Test
{
    [TestFixture]
    public class PatientsControllerTest
    {
        [Test]
        public void ControllerReturnsSinglePatientAndAssociatedEpisode()
        {
            var container = UnityConfig.GetConfiguredContainer();

            container.RegisterInstance<IDatabaseContext>(CreateMockContext());

            PatientsController patientsController = container.Resolve<PatientsController>();

            var patient = patientsController.Get(5);

            Assert.IsNotNull(patient);
            Assert.IsNotNull(patient.Episodes);
            Assert.IsTrue(patient.Episodes.ToList().Count == 1);

            container.Dispose();
        }

        [Test]
        public void ControllerThrows404ErrorWhenPatientIdDoesNotExist()
        {
            var container = UnityConfig.GetConfiguredContainer();

            container.RegisterInstance<IDatabaseContext>(CreateMockContext());

            PatientsController patientsController = container.Resolve<PatientsController>();

            var ex = Assert.Throws<HttpResponseException>(() => patientsController.Get(6));
            Assert.AreEqual(HttpStatusCode.NotFound, ex.Response.StatusCode);
            
            container.Dispose();
        }

        private IDatabaseContext CreateMockContext()
        {
            var mockContext = new InMemoryPatientContext();

            var patients = new List<Patient>
                {
                    new Patient
                        {
                            DateOfBirth = new DateTime(1972, 10, 27),
                            FirstName = "Dave",
                            PatientId = 4,
                            LastName = "Smith",
                            NhsNumber = "1111111111"
                        },
                    new Patient
                        {
                            DateOfBirth = new DateTime(1987, 2, 14),
                            FirstName = "Richard",
                            PatientId = 5,
                            LastName = "Higgins",
                            NhsNumber = "2222222222"
                        }
                };

            patients.ForEach(s => mockContext.Patients.Add(s));

            var episodes = new List<Episode>
                {
                    new Episode
                        {
                            AdmissionDate = new DateTime(2014, 11, 12),
                            Diagnosis = "Irritation of inner ear",
                            DischargeDate = new DateTime(2014, 11, 27),
                            EpisodeId = 6,
                            PatientId = 4
                        },
                    new Episode
                        {
                            AdmissionDate = new DateTime(2015, 3, 20),
                            Diagnosis = "Sprained wrist",
                            DischargeDate = new DateTime(2015, 4, 2),
                            EpisodeId = 7,
                            PatientId = 4
                        },
                    new Episode
                        {
                            AdmissionDate = new DateTime(2015, 11, 12),
                            Diagnosis = "Stomach cramps",
                            DischargeDate = new DateTime(2015, 11, 14),
                            EpisodeId = 8,
                            PatientId = 4
                        },
                    new Episode
                        {
                            AdmissionDate = new DateTime(2015, 4, 18),
                            Diagnosis = "Laryngitis",
                            DischargeDate = new DateTime(2015, 5, 26),
                            EpisodeId = 9,
                            PatientId = 5
                        }
                };
            episodes.ForEach(s => mockContext.Episodes.Add(s));

            return mockContext;
        }
    }
}
