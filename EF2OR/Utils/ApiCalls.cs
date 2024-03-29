﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using EF2OR.ViewModels;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using EF2OR.Enums;
using EF2OR.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using System.Configuration;
using StaffNS = EF2OR.Entities.EdFiOdsApi.Enrollment.Staffs;
using SchoolsNS = EF2OR.Entities.EdFiOdsApi.Enrollment.Schools;
using SectionsNS = EF2OR.Entities.EdFiOdsApi.Enrollment.Sections;
using StudentsNS = EF2OR.Entities.EdFiOdsApi.Enrollment.Students;
using EF2OR.Entities.EdFiOdsApi;
using SectionEnrollmentsNS = EF2OR.Entities.EdFiOdsApi.Enrollment.SectionEnrollments;
using ParentNS = EF2OR.Entities.EdFiOdsApi.Resourses.Parent;
using StudentParentAssociationNS = EF2OR.Entities.EdFiOdsApi.Resourses.StudentParentAssociation;
using StudentSchoolAssociationNS = EF2OR.Entities.EdFiOdsApi.Resourses.StudentSchoolAssociation;
using StudentSectionAssociationNS = EF2OR.Entities.EdFiOdsApi.Resourses.StudentSectionAssociation;
using CourseOfferingNS = EF2OR.Entities.EdFiOdsApi.Resourses.CourseOffering;

namespace EF2OR.Utils
{
    public class ApiCalls
    {
        #region Variables
        private static readonly ApplicationDbContext db = new ApplicationDbContext();


        private static List<CsvUsers> _staffList;
        private static List<CsvUsers> _studList;
        static ApiCalls()
        {
            Providers.ApiResponseProvider.db = ApiCalls.db;
            CommonUtils.ExistingResponses = new Dictionary<string, Entities.EdFiOdsApi.IEdFiOdsData>();
        }
        private static string UserId
        {
            get
            {
                return CommonUtils.UserProvider.UserId;
            }
        }

        private static int _checkboxPageSize = Convert.ToInt32(ConfigurationManager.AppSettings["CheckboxPageSize"]);
        private static int _maxApiCallSize = Convert.ToInt32(ConfigurationManager.AppSettings["ApiMaxCallSize"]);
        private static int _dataPreviewPageSize = Convert.ToInt32(ConfigurationManager.AppSettings["DataPreviewPageSize"]);

        #endregion


        #region FilterMethods
        public static async Task PopulateFilterSection1(ExportsViewModel model)
        {
            var schools = await GetSchools("id,nameOfInstitution");
            //var schoolYears = await GetSchoolYears("id,uniqueSectionCode,academicSubjectDescriptor,sessionReference,courseOfferingReference,locationReference,schoolReference,staff");
            //var terms = await GetTerms();

            model.SchoolsCriteriaSection = new ApiCriteriaSection
            {
                FilterCheckboxes = schools,
                SectionName = "Schools",
                IsExpanded = true,
                AllDataReceived = true
            };

            //model.SchoolYearsCriteriaSection = new ApiCriteriaSection
            //{
            //    FilterCheckboxes = schoolYears,
            //    SectionName = "School Years",
            //    IsExpanded = true,
            //    AllDataReceived = true
            //};

            //model.TermsCriteriaSection = new ApiCriteriaSection
            //{
            //    FilterCheckboxes = terms,
            //    SectionName = "Terms",
            //    IsExpanded = true,
            //    AllDataReceived = true
            //};
        }

        public static async Task<List<ExportsCheckbox>> GetSchools(string fields = null)
        {
            if (CommonUtils.HttpContextProvider.Current.Session["AllSchools"] == null)
            {
                var responseArray = await CommonUtils.ApiResponseProvider.GetApiData<SchoolsNS.Schools>(ApiEndPoints.Schools, false, fields) as SchoolsNS.Schools;
                var schools = (from s in responseArray.Property1
                               select new ExportsCheckbox
                               {
                                   Id = s.id,
                                   SchoolId = Convert.ToString(s.schoolId),
                                   Text = s.nameOfInstitution,
                                   Visible = true
                               }).OrderBy(x => x.Text).ToList();

                CommonUtils.HttpContextProvider.Current.Session["AllSchools"] = schools;
            }

            var allSchools = (List<ExportsCheckbox>)CommonUtils.HttpContextProvider.Current.Session["AllSchools"];
            allSchools.ForEach(c => c.Selected = false); // make sure all are unchecked first

            return allSchools;
        }

        //public static async Task<List<ExportsCheckbox>> GetSchoolYears(string fields = null)
        //{
        //    if (CommonUtils.HttpContextProvider.Current.Session["AllSchoolYears"] == null)
        //    {
        //        var responseArray = await CommonUtils.ApiResponseProvider.GetApiData<SectionsNS.Sections>(ApiEndPoints.SchoolYears, false, fields) as SectionsNS.Sections;

        //        var schoolYearsStrings = (from s in responseArray.Property1
        //                                  select Convert.ToString(s.sessionReference.schoolYear)).Distinct();

        //        var schoolYears = (from s in schoolYearsStrings
        //                           select new ExportsCheckbox
        //                           {
        //                               Id = s,
        //                               Text = s,
        //                               Visible = true
        //                           }).OrderBy(x => x.Text).ToList();
        //        CommonUtils.HttpContextProvider.Current.Session["AllSchoolYears"] = schoolYears;
        //    }

        //    var allSchoolYears = (List<ExportsCheckbox>)CommonUtils.HttpContextProvider.Current.Session["AllSchoolYears"];
        //    allSchoolYears.ForEach(c => c.Selected = false); // make sure all are unchecked first

        //    return allSchoolYears;
        //}

        //public static async Task<List<ExportsCheckbox>> GetTerms()
        //{
        //    if (CommonUtils.HttpContextProvider.Current.Session["AllTerms"] == null)
        //    {
        //        var responseArray = await CommonUtils.ApiResponseProvider.GetApiData<SectionsNS.Sections>(ApiEndPoints.SchoolYears) as SectionsNS.Sections;
        //        var termStrings = (from s in responseArray.Property1
        //                           select Convert.ToString(s.sessionReference.termDescriptor)).Distinct();

        //        var terms = (from s in termStrings
        //                     select new ExportsCheckbox
        //                     {
        //                         Id = s,
        //                         Text = s,
        //                         Visible = true
        //                     }).OrderBy(x => x.Text).ToList();
        //        CommonUtils.HttpContextProvider.Current.Session["AllTerms"] = terms;
        //    }

        //    var allTerms = (List<ExportsCheckbox>)CommonUtils.HttpContextProvider.Current.Session["AllTerms"];
        //    allTerms.ForEach(c => c.Selected = false); // make sure all are unchecked first

        //    return allTerms;
        //}

        //public static async Task<ApiCriteriaSection> GetSubjects(List<string> schoolIds,
        //    List<string> schoolYears,
        //    List<string> terms,
        //    bool getMore)
        //{
        //    string sectionName = "Subjects";
        //    var model = GetSessionModel(sectionName, getMore);

        //    while (model.FilterCheckboxes.Count() < model.NumCheckBoxesToDisplay && !model.AllDataReceived)
        //    {
        //        var responseArray = await CommonUtils.ApiResponseProvider.GetPagedApiData(ApiEndPoints.Subjects, model.CurrentOffset);
        //        var checkboxes = (from s in responseArray
        //                          select new ExportsCheckbox
        //                        {
        //                            Id = (string)s["academicSubjectDescriptor"],
        //                            SchoolId = (string)s["schoolReference"]["id"],
        //                            SchoolYear = (string)s["sessionReference"]["schoolYear"],
        //                            Term = (string)s["sessionReference"]["termDescriptor"],
        //                            Text = (string)s["academicSubjectDescriptor"],
        //                            Visible = true
        //                        }).OrderBy(x => x.Text).ToList();

        //        if (checkboxes.Count < _maxApiCallSize)
        //        {
        //            model.AllDataReceived = true;
        //        }

        //        model.CurrentOffset += _maxApiCallSize;
        //        model.AllCheckboxes.AddRange(checkboxes);
        //        model.FilterCheckboxes = FilterCheckboxes(model.AllCheckboxes, schoolIds, schoolYears, terms);
        //    }

        //    model.FilterCheckboxes = model.FilterCheckboxes.Take(model.NumCheckBoxesToDisplay).ToList();
        //    model.FilterCheckboxes.ForEach(c => c.Selected = false); // make sure all are unchecked first

        //    CommonUtils.HttpContextProvider.Current.Session["SubjectsModel"] = model;

        //    return model;
        //}

        //public static async Task<ApiCriteriaSection> GetCourses(List<string> schoolIds,
        //    List<string> schoolYears,
        //    List<string> terms,
        //    bool getMore)
        //{
        //    string sectionName = "Courses";
        //    var model = GetSessionModel(sectionName, getMore);

        //    while (model.FilterCheckboxes.Count() < model.NumCheckBoxesToDisplay && !model.AllDataReceived)
        //    {
        //        var responseArray = await CommonUtils.ApiResponseProvider.GetPagedApiData(ApiEndPoints.Courses, model.CurrentOffset);
        //        var checkboxes = (from s in responseArray
        //                        select new ExportsCheckbox
        //                        {
        //                            Id = (string)s["courseOfferingReference"]["localCourseCode"],
        //                            SchoolId = (string)s["schoolReference"]["id"],
        //                            SchoolYear = (string)s["sessionReference"]["schoolYear"],
        //                            Term = (string)s["sessionReference"]["termDescriptor"],
        //                            Text = (string)s["courseOfferingReference"]["localCourseCode"],
        //                            Subject = (string)s["academicSubjectDescriptor"],
        //                            Visible = true
        //                        }).OrderBy(x => x.Text).ToList();

        //        if (checkboxes.Count < _maxApiCallSize)
        //        {
        //            model.AllDataReceived = true;
        //        }

        //        model.CurrentOffset += _maxApiCallSize;
        //        model.AllCheckboxes.AddRange(checkboxes);
        //        model.FilterCheckboxes = FilterCheckboxes(model.AllCheckboxes, schoolIds, schoolYears, terms);
        //    }

        //    model.FilterCheckboxes = model.FilterCheckboxes.Take(model.NumCheckBoxesToDisplay).ToList();
        //    model.FilterCheckboxes.ForEach(c => c.Selected = false); // make sure all are unchecked first

        //    CommonUtils.HttpContextProvider.Current.Session[sectionName + "Model"] = model;

        //    return model;
        //}
        public static async Task<List<ExportsCheckbox>> GetSpecificTeachers(List<string> ids)
        {
            var teachers = new List<ExportsCheckbox>();
            if (ids != null)
                foreach (var id in ids)
                {
                    var endpoint = string.Format(ApiEndPoints.StaffWithId, id);
                    var staffResponse = await CommonUtils.ApiResponseProvider.GetPagedApiData(endpoint, 0, "id,firstName,lastSurname");
                    var oneTeacher = (from s in staffResponse
                                      select new ExportsCheckbox
                                      {
                                          Id = (string)s["id"],
                                          Text = (string)s["firstName"] + " " + (string)s["lastSurname"],
                                          Selected = true
                                      }).FirstOrDefault();
                    teachers.Add(oneTeacher);
                }
            return teachers;
        }

        public static async Task<ApiCriteriaSection> GetTeachers(List<string> schoolIds,
            bool getMore)
        {
            string sectionName = "Teachers";
            var model = GetSessionModel(sectionName, getMore, schoolIds);

            if (model.SchoolIds != null && model.SchoolIds.Count() > 0)
            {
                if (string.IsNullOrEmpty(model.CurrentSchoolId))
                {
                    model.CurrentSchoolId = model.SchoolIds[0];
                }

                var endpoint = string.Format(ApiEndPoints.StaffWithSchoolId, model.CurrentSchoolId);
                await GetTeachersData(model, endpoint, schoolIds);

                if (model.CurrentSchoolAllDataReceived)
                {
                    var currentSchoolIndex = model.SchoolIds.FirstOrDefault(x => x.Value == model.CurrentSchoolId).Key;
                    if (model.SchoolIds.ContainsKey(currentSchoolIndex + 1))
                    {
                        model.CurrentSchoolAllDataReceived = false;
                        model.CurrentOffset = 0;
                        model.CurrentSchoolId = model.SchoolIds[currentSchoolIndex + 1];
                    }
                    else
                    {
                        model.AllDataReceived = true;
                    }
                }
            }
            else
            {
                await GetTeachersData(model, ApiEndPoints.Staff, schoolIds);
            }

            model.FilterCheckboxes = model.AllCheckboxes.Take(model.NumCheckBoxesToDisplay).ToList();
            model.FilterCheckboxes.ForEach(c => c.Selected = false); // make sure all are unchecked first
            model.CanGetMore = !model.AllDataReceived || model.FilterCheckboxes.Count() < model.AllCheckboxes.Count();

            CommonUtils.HttpContextProvider.Current.Session[sectionName + "Model"] = model;

            return model;
        }

        private static async Task GetTeachersData(ApiCriteriaSection model, string endpoint, List<string> schoolIds)
        {
            while (model.AllCheckboxes.Count() < model.NumCheckBoxesToDisplay && !model.AllDataReceived && !model.CurrentSchoolAllDataReceived)
            {
                var staffResponse = await CommonUtils.ApiResponseProvider.GetPagedApiData(endpoint, model.CurrentOffset, "id,firstName,lastSurname");// as StaffNS.Staffs;
                var staffs = (from s in staffResponse
                              select new ExportsCheckbox
                              {
                                  Id = (string)s["id"],
                                  Text = (string)s["firstName"] + " " + (string)s["lastSurname"]
                              }).ToList();

                if (staffs.Count() < _maxApiCallSize)
                {
                    if (model.SchoolIds != null && model.SchoolIds.Count() > 0)
                    {
                        model.CurrentSchoolAllDataReceived = true;
                    }
                    else
                    {
                        model.AllDataReceived = true;
                    }
                }

                model.CurrentOffset += _maxApiCallSize;
                model.AllCheckboxes.AddRange(staffs);
            }
        }

        public static async Task<List<ExportsCheckbox>> GetSpecificSections(List<string> ids)
        {
            var sections = new List<ExportsCheckbox>();
            foreach (var id in ids)
            {
                var endpoint = string.Format(ApiEndPoints.Sections, id);
                var fields = "uniqueSectionCode";
                var additionalParam = "&uniqueSectionCode=" + id;
                var sectionsResponse = await CommonUtils.ApiResponseProvider.GetPagedApiData(endpoint, 0, fields + additionalParam);
                var oneSection = (from s in sectionsResponse
                                  select new ExportsCheckbox
                                  {
                                      Id = (string)s["uniqueSectionCode"],
                                      Text = (string)s["uniqueSectionCode"],
                                      Selected = true
                                  }).FirstOrDefault();
                sections.Add(oneSection);
            }
            return sections;
        }

        public static async Task<ApiCriteriaSection> GetSections(List<string> schoolIds,
            bool getMore)
        {
            string sectionName = "Sections";
            var model = GetSessionModel(sectionName, getMore, schoolIds);

            if (model.SchoolIds != null && model.SchoolIds.Count() > 0)
            {
                if (string.IsNullOrEmpty(model.CurrentSchoolId))
                {
                    model.CurrentSchoolId = model.SchoolIds[0];
                }

                var endpoint = string.Format(ApiEndPoints.SectionsWithSchoolId, model.CurrentSchoolId);
                await GetSectionsData(model, endpoint, schoolIds);

                if (model.CurrentSchoolAllDataReceived)
                {
                    var currentSchoolIndex = model.SchoolIds.FirstOrDefault(x => x.Value == model.CurrentSchoolId).Key;
                    if (model.SchoolIds.ContainsKey(currentSchoolIndex + 1))
                    {
                        model.CurrentSchoolAllDataReceived = false;
                        model.CurrentOffset = 0;
                        model.CurrentSchoolId = model.SchoolIds[currentSchoolIndex + 1];
                    }
                    else
                    {
                        model.AllDataReceived = true;
                    }
                }
            }
            else
            {
                await GetSectionsData(model, ApiEndPoints.Sections, schoolIds);
            }

            model.FilterCheckboxes = model.AllCheckboxes.Take(model.NumCheckBoxesToDisplay).ToList();
            model.FilterCheckboxes.ForEach(c => c.Selected = false); // make sure all are unchecked first
            model.CanGetMore = !model.AllDataReceived || model.FilterCheckboxes.Count() < model.AllCheckboxes.Count();

            CommonUtils.HttpContextProvider.Current.Session[sectionName + "Model"] = model;

            return model;
        }

        private static async Task GetSectionsData(ApiCriteriaSection model, string endpoint, List<string> schoolIds)
        {
            while (model.AllCheckboxes.Count() < model.NumCheckBoxesToDisplay && !model.AllDataReceived && !model.CurrentSchoolAllDataReceived)
            {
                var sectionsResponse = await CommonUtils.ApiResponseProvider.GetPagedApiData(endpoint, model.CurrentOffset);
                var sections = (from s in sectionsResponse
                                select new ExportsCheckbox
                                {
                                    Id = (string)s["uniqueSectionCode"],
                                    Text = (string)s["uniqueSectionCode"]
                                }).ToList();

                if (sections.Count() < _maxApiCallSize)
                {
                    if (model.SchoolIds != null && model.SchoolIds.Count() > 0)
                    {
                        model.CurrentSchoolAllDataReceived = true;
                    }
                    else
                    {
                        model.AllDataReceived = true;
                    }
                }

                model.CurrentOffset += _maxApiCallSize;
                model.AllCheckboxes.AddRange(sections);
            }
        }

        private static ApiCriteriaSection GetSessionModel(string sectionName, bool getMore, List<string> schoolIds)
        {
            var model = new ApiCriteriaSection();
            if (CommonUtils.HttpContextProvider.Current.Session[sectionName + "Model"] != null && getMore)
            {
                model = (ApiCriteriaSection)CommonUtils.HttpContextProvider.Current.Session[sectionName + "Model"];
                model.NumCheckBoxesToDisplay = model.NumCheckBoxesToDisplay + _checkboxPageSize;
            }
            else
            {
                model = new ApiCriteriaSection
                {
                    SectionName = sectionName,
                    IsExpanded = true,
                    CurrentOffset = 0,
                    NumCheckBoxesToDisplay = _checkboxPageSize,
                    FilterCheckboxes = new List<ExportsCheckbox>(),
                    AllCheckboxes = new List<ExportsCheckbox>(),
                    CanGetMore = true
                };

                if (schoolIds != null && schoolIds.Count() > 0)
                {
                    model.SchoolIds = new Dictionary<int, string>();
                    for (var i = 0; i < schoolIds.Count(); i++)
                    {
                        model.SchoolIds.Add(i, schoolIds[i]);
                    }
                }
            }
            return model;
        }

        //private static List<ExportsCheckbox> FilterCheckboxes(List<ExportsCheckbox> allBoxes,
        //   List<string> schoolIds)
        //   //List<string> schoolYears,
        //   //List<string> terms)
        //{
        //    bool allSchools = schoolIds == null || schoolIds.Count() == 0;
        //    //bool allSchoolYears = schoolYears == null || schoolYears.Count() == 0;
        //    //bool allTerms = terms == null || terms.Count() == 0;

        //    var filteredBoxes = new List<ExportsCheckbox>();
        //    filteredBoxes.AddRange(allBoxes);

        //    if (!allSchools)
        //    {
        //        filteredBoxes = filteredBoxes.Where(x =>
        //        schoolIds.Contains(x.SchoolId)).ToList();
        //    }

        //    //if (!allSchoolYears)
        //    //{
        //    //    filteredBoxes = filteredBoxes.Where(x =>
        //    //    schoolYears.Contains(x.SchoolYear)).ToList();
        //    //}

        //    //if (!allTerms)
        //    //{
        //    //    filteredBoxes = filteredBoxes.Where(x =>
        //    //    terms.Contains(x.Term)).ToList();
        //    //}

        //    filteredBoxes = filteredBoxes.GroupBy(x => x.Text).Select(group => group.First()).ToList();

        //    return filteredBoxes;
        //}

        #endregion

        #region ResultsMethods
        public static async Task<List<string>> GetTermDescriptors(bool forceNew = false)
        {
            var responseArray = await CommonUtils.ApiResponseProvider.GetApiData<TermDescriptors>(ApiEndPoints.TermDescriptors, forceNew) as TermDescriptors;
            var terms = responseArray.Property1.Select(x => x.description).Distinct();
            CommonUtils.ExistingResponses.Remove(ApiEndPoints.TermDescriptors);  //now we have one in there with only termDescriptor
            return terms.ToList();
        }

        public static async Task<PreveiwJsonResults> GetJsonPreviews(FilterInputs inputs, string oneRosterVersion)
        {
            var dataResults = await GetDataForPreview(inputs, oneRosterVersion);
            //var dataResults = await GetDataResults(inputs, oneRosterVersion);

            var previewModel = new PreveiwJsonResults();
            previewModel.Orgs = JsonConvert.SerializeObject(dataResults.Orgs);
            previewModel.Users = JsonConvert.SerializeObject(dataResults.Users);
            previewModel.Courses = JsonConvert.SerializeObject(dataResults.Courses);
            previewModel.Classes = JsonConvert.SerializeObject(dataResults.Classes);
            previewModel.Enrollments = JsonConvert.SerializeObject(dataResults.Enrollments);
            previewModel.AcademicSessions = JsonConvert.SerializeObject(dataResults.AcademicSessions);

            previewModel.OrgsTotalPages = dataResults.Orgs.Count() < _dataPreviewPageSize ? 1 : 0;
            previewModel.UsersTotalPages = dataResults.Users.Count() < _dataPreviewPageSize ? 1 : 0;
            previewModel.CoursesTotalPages = dataResults.Courses.Count() < _dataPreviewPageSize ? 1 : 0;
            previewModel.ClassesTotalPages = dataResults.Classes.Count() < _dataPreviewPageSize ? 1 : 0;
            previewModel.EnrollmentsTotalPages = dataResults.Enrollments.Count() < _dataPreviewPageSize ? 1 : 0;
            previewModel.AcademicSessionsTotalPages = dataResults.AcademicSessions.Count() < _dataPreviewPageSize ? 1 : 0;

            if (oneRosterVersion == OneRosterVersions.OR_1_1)
            {
                previewModel.Manifest = JsonConvert.SerializeObject(dataResults.Manifest);
            }

            return previewModel;
        }

        public static async Task<DataResults> GetDataResults(FilterInputs inputs, string oneRosterVersion)
        {
            var dataResults = new DataResults();
            if (CommonUtils.ExistingResponses.Count() > 0)
            {
                CommonUtils.ExistingResponses.Clear(); //reset the global dictionary so we get fresh data
            }

            dataResults.Orgs = await GetCsvOrgs(inputs);
            //Insert School Id Nos
            foreach (CsvOrgs s in dataResults.Orgs)
                inputs.SchoolIds.Add(s.identifier);
            //dataResults.Users = await GetCsvUsers(inputs);
            dataResults.Courses = await GetCsvCourses(inputs);
            dataResults.Classes = await GetCsvClasses(inputs);
            dataResults.Enrollments = await GetCsvEnrollments(inputs);
            dataResults.AcademicSessions = await GetCsvAcademicSessions(inputs);
            dataResults.Demographics = await GetCsvDemographics(inputs);


            //dataResults.Orgs = new List<CsvOrgs>();
            //dataResults.Courses = new List<CsvCourses>();
            //dataResults.Classes = new List<CsvClasses>();
            //dataResults.Enrollments = new List<CsvEnrollments>();
            //dataResults.AcademicSessions = new List<CsvAcademicSessions>();
            if (oneRosterVersion == OneRosterVersions.OR_1_1)
            {
                dataResults.Manifest = GetCsvManifest(DownloadTypes.bulk);
            }
            CommonUtils.ExistingResponses.Clear(); //reset the global dictionary so next time we get fresh data.  Also so it doesn't sit in memory.
            return dataResults;
        }

        public static async Task<DataPreviewPagedJsonModel> GetPreviewOrgsJsonString(int pageNumber)
        {
            var data = await GetPagedOrgs(null, pageNumber);
            var serializedList = JsonConvert.SerializeObject(data.OrgsCurrentPage);
            return new DataPreviewPagedJsonModel
            {
                JsonData = serializedList,
                CurrentPage = pageNumber,
                TotalPages = data.TotalPages
            };
        }

        public static async Task<DataPreviewPagedJsonModel> GetPreviewUsersJsonString(int pageNumber)
        {
            var data = await GetPagedUsers(null, pageNumber);
            var serializedList = JsonConvert.SerializeObject(data.UsersCurentPage);
            return new DataPreviewPagedJsonModel
            {
                JsonData = serializedList,
                CurrentPage = pageNumber,
                TotalPages = data.TotalPages
            };
        }

        public static async Task<DataPreviewPagedJsonModel> GetPreviewCoursesJsonString(int pageNumber)
        {
            var data = await GetPagedCourses(null, pageNumber);
            var serializedList = JsonConvert.SerializeObject(data.CoursesCurentPage);
            return new DataPreviewPagedJsonModel
            {
                JsonData = serializedList,
                CurrentPage = pageNumber,
                TotalPages = data.TotalPages
            };
        }

        public static async Task<DataPreviewPagedJsonModel> GetPreviewClassesJsonString(int pageNumber)
        {
            var data = await GetPagedClasses(null, pageNumber);
            var serializedList = JsonConvert.SerializeObject(data.ClassesCurentPage);
            return new DataPreviewPagedJsonModel
            {
                JsonData = serializedList,
                CurrentPage = pageNumber,
                TotalPages = data.TotalPages
            };
        }

        public static async Task<DataPreviewPagedJsonModel> GetPreviewEnrollmentsJsonString(int pageNumber)
        {
            var data = await GetPagedEnrollments(null, pageNumber);
            var serializedList = JsonConvert.SerializeObject(data.EnrollmentsCurentPage);
            return new DataPreviewPagedJsonModel
            {
                JsonData = serializedList,
                CurrentPage = pageNumber,
                TotalPages = data.TotalPages
            };
        }

        public static async Task<DataPreviewPagedJsonModel> GetPreviewAcademicSessionsJsonString(int pageNumber)
        {
            var data = await GetPagedAcademicSessions(null, pageNumber);
            var serializedList = JsonConvert.SerializeObject(data.AcademicSessionsCurrentPage);
            return new DataPreviewPagedJsonModel
            {
                JsonData = serializedList,
                CurrentPage = pageNumber,
                TotalPages = data.TotalPages
            };
        }


        public static async Task<DataResults> GetDataForPreview(FilterInputs inputs, string oneRosterVersion)
        {
            var dataResults = new DataResults();

            if (CommonUtils.ExistingResponses.Count() > 0)
            {
                CommonUtils.ExistingResponses.Clear(); //reset the global dictionary so we get fresh data
            }

            //dataResults.Orgs = await GetCsvOrgs(inputs);
            //dataResults.Users = await GetCsvUsers(inputs);
            //dataResults.Courses = await GetCsvCourses(inputs);
            //dataResults.Classes = await GetCsvClasses(inputs);
            //dataResults.Enrollments = await GetCsvEnrollments(inputs);
            //dataResults.AcademicSessions = await GetCsvAcademicSessions(inputs);
            var orgsPage1 = (await GetPagedOrgs(inputs, 0)).OrgsCurrentPage;
            var usersPage1 = (await GetPagedUsers(inputs, 0)).UsersCurentPage;
            var coursesPage1 = (await GetPagedCourses(inputs, 0)).CoursesCurentPage;
            var classesPage1 = (await GetPagedClasses(inputs, 0)).ClassesCurentPage;
            var enrollmentsPage1 = (await GetPagedEnrollments(inputs, 0)).EnrollmentsCurentPage;
            var academicSessionsPage1 = (await GetPagedAcademicSessions(inputs, 0)).AcademicSessionsCurrentPage;
            //var usersPage1 = (await GetPagedUsers(inputs, 0)).UsersCurentPage;
            dataResults.Orgs = orgsPage1;
            dataResults.Users = usersPage1;
            dataResults.Courses = coursesPage1;
            dataResults.Classes = classesPage1;
            dataResults.Enrollments = enrollmentsPage1;
            dataResults.AcademicSessions = academicSessionsPage1;
            if (oneRosterVersion == OneRosterVersions.OR_1_1)
            {
                dataResults.Manifest = GetCsvManifest(DownloadTypes.bulk);
            }

            CommonUtils.ExistingResponses.Clear(); //reset the global dictionary so next time we get fresh data.  Also so it doesn't sit in memory.

            return dataResults;
            //
        }

        private static async Task<List<CsvOrgs>> GetCsvOrgs(FilterInputs inputs)
        {
            var responseArray = await CommonUtils.ApiResponseProvider.GetApiData<SchoolsNS.Schools>(ApiEndPoints.CsvOrgs) as SchoolsNS.Schools;
            var context = new ApplicationDbContext();
            var identifierSetting = context.ApplicationSettings.FirstOrDefault(x => x.SettingName == ApplicationSettingsTypes.OrgsIdentifier)?.SettingValue;
            bool blankIdentifier = identifierSetting == null || identifierSetting == OrgIdentifierSettings.blank;
            context.Dispose();

            var listOfObjects = (from o in responseArray.Property1
                                 select new CsvOrgs
                                 {
                                     sourcedId = o.id,
                                     name = o.nameOfInstitution,
                                     type = "school",
                                     //identifier = blankIdentifier ? "" : o.stateOrganizationId,// Commented Krishanraj to Fix Student Data Need this Field
                                     identifier = o.schoolId.ToString(),//School Required for Student Data..
                                     parentSourcedId = (string)o.localEducationAgencyReference.id,
                                     SchoolId = (string)o.id
                                 });

            if (inputs != null && inputs.Schools != null)
            {
                listOfObjects = listOfObjects.Where(x => inputs.Schools.Contains(x.SchoolId));
            }
            return listOfObjects.ToList();
        }

        private static async Task<PagedDataResults> GetPagedOrgs(FilterInputs inputs, int pageNumber)
        {
            var model = new PagedDataResults();
            if (pageNumber != 0)
            {
                model = (PagedDataResults)CommonUtils.HttpContextProvider.Current.Session["OrgsDataPreviewResults"];
            }
            else
            {
                pageNumber = 1;
                model.Orgs = new List<CsvOrgs>();
                model.Inputs = inputs;
                CommonUtils.HttpContextProvider.Current.Session["OrgsDataPreviewResults"] = model;
            }

            bool getMore = model.Orgs == null || model.Orgs.Count() < (pageNumber * _dataPreviewPageSize);
            getMore = getMore && model.TotalPages == 0; //total pages is 0 until we got them all and know the max
            while (getMore)
            {
                var responseArray = await CommonUtils.ApiResponseProvider.GetPagedApiData(ApiEndPoints.CsvOrgs, model.CurrentOffset);

                var context = new ApplicationDbContext();
                var identifierSetting = context.ApplicationSettings.FirstOrDefault(x => x.SettingName == ApplicationSettingsTypes.OrgsIdentifier)?.SettingValue;
                bool blankIdentifier = identifierSetting == null || identifierSetting == OrgIdentifierSettings.blank;

                var orgsList = (from o in responseArray
                                select new CsvOrgs
                                {
                                    //sourcedId = (string)o["sessionReference"]["id"],
                                    sourcedId = (string)o["id"],
                                    name = (string)o["nameOfInstitution"],
                                    type = "school",
                                    identifier = blankIdentifier ? "" : (string)o["nameOfInstitution"],
                                    parentSourcedId = (string)o["localEducationAgencyReference"]["id"],
                                    SchoolId = (string)o["id"]
                                });

                var recordsReturnedFromApi = orgsList.Count();
                model.CurrentOffset += _maxApiCallSize;

                if (inputs != null && inputs.Schools != null)
                {
                    orgsList = orgsList.Where(x => inputs.Schools.Contains(x.SchoolId));
                }

                orgsList = orgsList.GroupBy(x => x.sourcedId).Select(group => group.First());//do we need to get all, then filter, then group by, then count?  I think so.  THe model mightneed another property for ALL academic session results...like how i did for checkboxes
                model.Orgs.AddRange(orgsList);

                if (recordsReturnedFromApi < _maxApiCallSize)
                {
                    getMore = false;
                    model.TotalPages = (model.Orgs.Count() + _dataPreviewPageSize - 1) / _dataPreviewPageSize; //http://stackoverflow.com/questions/17944/how-to-round-up-the-result-of-integer-division
                }

                if (model.Orgs.Count() >= (pageNumber * _dataPreviewPageSize))
                {
                    getMore = false;
                }
                context.Dispose();
            }

            var startAt = (pageNumber - 1) * _dataPreviewPageSize;
            model.OrgsCurrentPage = model.Orgs.Skip(startAt).Take(_dataPreviewPageSize).ToList();
            return model;
        }

        private static async Task<List<CsvUsers>> GetCsvUsers(FilterInputs inputs)
        {
            var enrollmentsResponse = await CommonUtils.ApiResponseProvider.GetApiData<SectionsNS.Sections>(ApiEndPoints.CsvUsers) as SectionsNS.Sections;

            var enrollmentsList = (from o in enrollmentsResponse.Property1
                                   let students = o.students.Select(x => x.id)
                                   let staffs = o.staff.Select(x => x)
                                   let teachers = o.staff.Select(x => x.id)
                                   select new
                                   {
                                       students = students,
                                       staffs = staffs,
                                       SchoolId = o.schoolReference.id,
                                       SchoolYear = Convert.ToString(o.courseOfferingReference.schoolYear),
                                       //Term = o.courseOfferingReference.termDescriptor,
                                       //Subject = o.academicSubjectDescriptor,
                                       //Course = o.courseOfferingReference.localCourseCode,
                                       Section = o.uniqueSectionCode,
                                       Teachers = teachers
                                   }).ToList();

            if (inputs != null)
            {
                if (inputs.Schools != null)
                    enrollmentsList = enrollmentsList.Where(x => inputs.Schools.Contains(x.SchoolId)).ToList();

                //if (inputs.Sections != null)
                //    enrollmentsList = enrollmentsList.Where(x => inputs.Sections.Contains(x.Section)).ToList();

                if (inputs.Teachers != null)
                    enrollmentsList = enrollmentsList.Where(x => x.Teachers.Intersect(inputs.Teachers).Any()).ToList();

                if (inputs.SchoolYears != null)
                    enrollmentsList = enrollmentsList.Where(x => inputs.SchoolYears.Contains(x.SchoolYear)).ToList();
            }

            var studentSchoolAssoctionResponse = await CommonUtils.ApiResponseProvider.GetApiData<StudentSchoolAssociationNS.StudentSchoolAssociation>(ApiEndPoints.CsvStudentSchoolAssociation) as StudentSchoolAssociationNS.StudentSchoolAssociation;
            var studentSchoolAssoctionList = (from o in studentSchoolAssoctionResponse.Property1
                                              select new
                                              {
                                                  id = o.id,
                                                  StudetUniqueId = o.studentReference.studentUniqueId,
                                                  SchoolId = o.schoolReference.schoolId,
                                                  SchoolYear = o.schoolYearTypeReference.schoolYear,
                                                  ExitWithdrawDate = o.exitWithdrawDate,
                                                  EntryDate = o.entryDate
                                              }).ToList();
            if (inputs != null)
            {
                if (inputs.Schools != null)
                    studentSchoolAssoctionList = studentSchoolAssoctionList.Where(x => inputs.SchoolIds.Contains(x.SchoolId)).ToList();
                if (inputs.SchoolYears != null)
                    studentSchoolAssoctionList = studentSchoolAssoctionList.Where(x => inputs.SchoolYears.Contains(x.SchoolYear)).ToList();
                studentSchoolAssoctionList = studentSchoolAssoctionList.Where(x => string.IsNullOrEmpty(x.ExitWithdrawDate)).ToList();
            }

            var studentsResponse = await CommonUtils.ApiResponseProvider.GetApiData<StudentsNS.Students>(ApiEndPoints.CsvUsersStudents) as StudentsNS.Students;
            var studentsResponseInfo = (from s in studentsResponse.Property1 //where s.sexType=="M"
                                                                             //let mainTelephone = (s.telephones == null || s.telephones.Count() == 0) ? null : s.telephones.FirstOrDefault(x => x.orderOfPriority == "1")
                                        let mainTelephone = (s.telephones == null || s.telephones.Count() == 0) ? null : s.telephones.FirstOrDefault()
                                        let mainTelephoneNumber = mainTelephone == null ? "" : (string)mainTelephone.telephoneNumber
                                        let emailAddress = (s.electronicMails == null || s.electronicMails.Count() == 0) ? "" : (string)s.electronicMails[0].electronicMailAddress //TODO: just pick 0?.  or get based on electronicMailType field.
                                        let mobile = (s.telephones == null || s.telephones.Count() == 0) ? null : s.telephones.FirstOrDefault(x => (string)x.telephoneNumberType == "Mobile")
                                        let mobileNumber = mobile == null ? "" : mobile.telephoneNumber
                                        select new
                                        {
                                            id = s.id,
                                            userId = s.studentUniqueId,
                                            givenName = s.firstName,
                                            familyName = s.lastSurname,
                                            middleName = s.middleName,
                                            identifier = s.studentUniqueId,
                                            email = emailAddress,
                                            sms = mobileNumber,
                                            phone = mainTelephoneNumber,
                                            username = s.loginId,
                                            //grade = s.schoolAssociations.FirstOrDefault().gradeLevel
                                            grade = GetHighestGrade(s.schoolAssociations)//Get highest Grade 
                                        }).ToList();


            var staffResponse = await CommonUtils.ApiResponseProvider.GetApiData<StaffNS.Staffs>(ApiEndPoints.CsvUsersStaff) as StaffNS.Staffs;
            var staffResponseInfo = (from s in staffResponse.Property1
                                         //let mainTelephone = (s.telephones == null || s.telephones.Count() == 0) ? null : s.telephones.FirstOrDefault(x => x.orderOfPriority == "1")
                                     let mainTelephone = (s.telephones == null || s.telephones.Count() == 0) ? null : s.telephones.FirstOrDefault()
                                     let mainTelephoneNumber = mainTelephone == null ? "" : mainTelephone.telephoneNumber
                                     let emailAddress = (s.electronicMails == null || s.electronicMails.Count() == 0) ? "" : s.electronicMails[0].electronicMailAddress //TODO: just pick 0?.  or get based on electronicMailType field.
                                     let mobile = (s.telephones == null || s.telephones.Count() == 0) ? null : s.telephones.FirstOrDefault(x => x.telephoneNumberType == "Mobile")
                                     let mobileNumber = mobile == null ? "" : mobile.telephoneNumber
                                     select new
                                     {
                                         id = s.id,
                                         userId = s.staffUniqueId,
                                         givenName = s.firstName,
                                         familyName = s.lastSurname,
                                         middleName = s.middleName,
                                         identifier = s.staffUniqueId,
                                         email = emailAddress,
                                         sms = mobileNumber,
                                         phone = mainTelephoneNumber,
                                         username = s.loginId

                                     }).ToList();

            var studentInfo = (from e in studentSchoolAssoctionList
                                   //from s in e.students
                               from si in studentsResponseInfo.Where(x => x.userId == e.StudetUniqueId)
                                   //from si in studentsResponseInfo.Where(x => x.id == s.id)
                               select new CsvUsers
                               {
                                   sourcedId = si.id,
                                   orgSourcedIds = e.SchoolId,
                                   enabledUser = "TRUE",
                                   role = "student",
                                   userId = si.userId,
                                   givenName = si.givenName,
                                   familyName = si.familyName,
                                   middleNames = si.middleName,
                                   identifier = si.identifier,
                                   email = si.email,
                                   sms = si.sms,
                                   phone = si.phone,
                                   username = si.username,
                                   grade = si.grade
                               }).ToList();

            var staffInfo = (from e in enrollmentsList
                             from s in e.staffs
                             from si in staffResponseInfo.Where(x => x.id == s.id)
                             select new CsvUsers
                             {
                                 sourcedId = s.id,
                                 orgSourcedIds = e.SchoolId,
                                 enabledUser = "TRUE",
                                 role = "teacher",
                                 userId = si.userId,
                                 givenName = si.givenName,
                                 familyName = si.familyName,
                                 middleNames = si.middleName,
                                 identifier = si.identifier,
                                 email = si.email,
                                 sms = si.sms,
                                 phone = si.phone,
                                 username = si.username
                             }).ToList();
            var studPrntAssoResponse = await CommonUtils.ApiResponseProvider.GetApiData<StudentParentAssociationNS.StudentParentAssociation>(ApiEndPoints.CsvStudPrntAsction) as StudentParentAssociationNS.StudentParentAssociation;
            var studPrntAssoInfo = (from o in studPrntAssoResponse.Property1
                                    select new
                                    {
                                        StudetUniqueId = o.studentReference.studentUniqueId,
                                        parentId = o.parentReference.parentUniqueId
                                        //SchoolId=o.parentReference.s
                                    }).ToList();

            var parentResponse = await CommonUtils.ApiResponseProvider.GetApiData<ParentNS.Parents>(ApiEndPoints.CsvUsersParents) as ParentNS.Parents;
            var parentResponseInfo = (from s in parentResponse.Property1
                                      let mainTelephone = (s.telephones == null || s.telephones.Count() == 0) ? null : s.telephones.FirstOrDefault()
                                      let mainTelephoneNumber = mainTelephone == null ? "" : (string)mainTelephone.telephoneNumber
                                      let emailAddress = (s.electronicMails == null || s.electronicMails.Count() == 0) ? "" : (string)s.electronicMails[0].electronicMailAddress
                                      let mobile = (s.telephones == null || s.telephones.Count() == 0) ? null : s.telephones.FirstOrDefault(x => (string)x.telephoneNumberType == "Mobile")
                                      let mobileNumber = mobile == null ? "" : mobile.telephoneNumber
                                      select new
                                      {
                                          id = s.id,
                                          userId = s.ParentUniqueId,
                                          givenName = s.firstName,
                                          familyName = s.lastSurname,
                                          middleName = s.middleName,
                                          identifier = s.ParentUniqueId,
                                          email = emailAddress,
                                          sms = mobileNumber,
                                          phone = mainTelephoneNumber,
                                          username = s.ParentUniqueId,
                                          grade = ""
                                      }).ToList();

            var parentInfo = (from pr in studPrntAssoInfo
                              from si in parentResponseInfo.Where(x => x.userId == pr.parentId)
                              from sii in studentInfo.Where(x => x.userId == pr.StudetUniqueId)
                              select new CsvUsers
                              {
                                  sourcedId = si.id,
                                  orgSourcedIds = sii.SchoolId,
                                  enabledUser = "TRUE",
                                  role = "Parent",
                                  userId = si.userId,
                                  givenName = si.givenName,
                                  familyName = si.familyName,
                                  middleNames = si.middleName,
                                  identifier = si.identifier,
                                  email = si.email,
                                  sms = si.sms,
                                  phone = si.phone,
                                  username = si.username
                              }).ToList();

            var distinctStudents = studentInfo.GroupBy(x => new { x.sourcedId, x.SchoolId }).Select(group => group.First()).ToList();
            var distinctStaff = staffInfo.GroupBy(x => new { x.sourcedId, x.SchoolId }).Select(group => group.First());
            var distinctParent = parentInfo.GroupBy(x => new { x.sourcedId, x.SchoolId }).Select(group => group.First()).ToList();

            var studentsAndStaff = distinctStudents.Concat(distinctStaff).ToList();
            var studentsAndStaffParent = studentsAndStaff.Concat(distinctParent).ToList();
            return studentsAndStaffParent.ToList();
        }

        internal static async Task<List<CsvCourses>> GetCsvCourses(FilterInputs inputs)
        {
            //var responseArray = await CommonUtils.ApiResponseProvider.GetApiData<SectionsNS.Sections>(ApiEndPoints.CsvCourses) as SectionsNS.Sections;
            //var enrollmentsList = (from o in responseArray.Property1
            //                       let teachers = o.staff.Select(x => x.id)
            //                       select new CsvCourses
            //                       {
            //                           sourcedId = o.courseOfferingReference.id,
            //                           schoolYearId = o.sessionReference.id,
            //                           title = o.courseOfferingReference.localCourseCode,
            //                           courseCode = o.courseOfferingReference.localCourseCode,
            //                           orgSourcedId = o.schoolReference.id,
            //                           subjects = o.academicSubjectDescriptor,
            //                           SchoolId = o.schoolReference.id,
            //                           SchoolYear = Convert.ToString(o.courseOfferingReference.schoolYear),
            //                           //Term = o.courseOfferingReference.termDescriptor,
            //                           //Subject = o.academicSubjectDescriptor,
            //                           //Course = o.courseOfferingReference.localCourseCode,
            //                           Section = o.uniqueSectionCode,
            //                           Teachers = teachers
            //                       }).ToList();

            var responseArray = await CommonUtils.ApiResponseProvider.GetApiData<CourseOfferingNS.CourseOffering>(ApiEndPoints.CsvCourseOffering) as CourseOfferingNS.CourseOffering;
            var enrollmentsList = (from o in responseArray.Property1
                                       //let teachers = o.staff.Select(x => x.id)
                                   select new CsvCourses
                                   {
                                       sourcedId = o.id,
                                       schoolYearId = o.sessionreference.schoolYear,
                                       title = o.localCourseTitle,
                                       courseCode = o.localCourseCode,
                                       orgSourcedId = o.schoolReference.schoolId,
                                       subjects = o.sessionreference.schoolYear,
                                       SchoolId = o.schoolReference.schoolId,
                                       SchoolYear = Convert.ToString(o.sessionreference.schoolYear),
                                       //Term = o.courseOfferingReference.termDescriptor,
                                       //Subject = o.academicSubjectDescriptor,
                                       //Course = o.courseOfferingReference.localCourseCode,
                                       Section = o.localCourseCode
                                       // Teachers = teachers
                                   }).ToList();


            if (inputs != null)
            {
                if (inputs.Schools != null)
                    enrollmentsList = enrollmentsList.Where(x => inputs.SchoolIds.Contains(x.SchoolId)).ToList();

                if (inputs.SchoolYears != null)
                    enrollmentsList = enrollmentsList.Where(x => inputs.SchoolYears.Contains(x.SchoolYear)).ToList();

                //if (inputs.Terms != null)
                //    enrollmentsList = enrollmentsList.Where(x => inputs.Terms.Contains(x.Term));

                //if (inputs.Subjects != null)
                //    enrollmentsList = enrollmentsList.Where(x => inputs.Subjects.Contains(x.Subject));

                //if (inputs.Courses != null)
                //    enrollmentsList = enrollmentsList.Where(x => inputs.Courses.Contains(x.Course));

                //if (inputs.Sections != null)
                //    enrollmentsList = enrollmentsList.Where(x => inputs.Sections.Contains(x.Section));

                //if (inputs.Teachers != null)
                //    enrollmentsList = enrollmentsList.Where(x => x.Teachers.Intersect(inputs.Teachers).Any());
            }

            enrollmentsList = enrollmentsList.GroupBy(x => x.title).Select(group => group.First()).ToList();

            return enrollmentsList.ToList();
        }

        private static async Task<List<CsvDemographics>> GetCsvDemographics(FilterInputs inputs)
        {
            var enrollmentsResponse = await CommonUtils.ApiResponseProvider.GetApiData<SectionsNS.Sections>(ApiEndPoints.CsvUsers) as SectionsNS.Sections;
            var enrollmentsList = (from o in enrollmentsResponse.Property1
                                   let students = o.students.Select(x => x.id)
                                   let staffs = o.staff.Select(x => x)
                                   let teachers = o.staff.Select(x => x.id)
                                   select new
                                   {
                                       students = students,
                                       staffs = staffs,
                                       SchoolId = o.schoolReference.id,
                                       SchoolYear = Convert.ToString(o.courseOfferingReference.schoolYear),
                                       //Term = o.courseOfferingReference.termDescriptor,
                                       //Subject = o.academicSubjectDescriptor,
                                       //Course = o.courseOfferingReference.localCourseCode,
                                       Section = o.uniqueSectionCode,
                                       Teachers = teachers
                                   }).ToList();

            if (inputs != null)
            {
                if (inputs.Schools != null)
                    enrollmentsList = enrollmentsList.Where(x => inputs.Schools.Contains(x.SchoolId)).ToList();
                //if (inputs.Sections != null)
                //    enrollmentsList = enrollmentsList.Where(x => inputs.Sections.Contains(x.Section)).ToList();
                if (inputs.Teachers != null)
                    enrollmentsList = enrollmentsList.Where(x => x.Teachers.Intersect(inputs.Teachers).Any()).ToList();
                if (inputs.SchoolYears != null)
                    enrollmentsList = enrollmentsList.Where(x => inputs.SchoolYears.Contains(x.SchoolYear)).ToList();
            }

            var studentSchoolAssoctionResponse = await CommonUtils.ApiResponseProvider.GetApiData<StudentSchoolAssociationNS.StudentSchoolAssociation>(ApiEndPoints.CsvStudentSchoolAssociation) as StudentSchoolAssociationNS.StudentSchoolAssociation;
            var studentSchoolAssoctionList = (from o in studentSchoolAssoctionResponse.Property1
                                              select new
                                              {
                                                  id = o.id,
                                                  StudetUniqueId = o.studentReference.studentUniqueId,
                                                  SchoolId = o.schoolReference.schoolId,
                                                  SchoolYear = o.schoolYearTypeReference.schoolYear,
                                                  ExitWithdrawDate = o.exitWithdrawDate,
                                                  EntryDate = o.entryDate
                                              }).ToList();
            if (inputs != null)
            {
                if (inputs.Schools != null)
                    studentSchoolAssoctionList = studentSchoolAssoctionList.Where(x => inputs.SchoolIds.Contains(x.SchoolId)).ToList();
                if (inputs.SchoolYears != null)
                    studentSchoolAssoctionList = studentSchoolAssoctionList.Where(x => inputs.SchoolYears.Contains(x.SchoolYear)).ToList();
                studentSchoolAssoctionList = studentSchoolAssoctionList.Where(x => string.IsNullOrEmpty(x.ExitWithdrawDate)).ToList();
            }

            var studentsResponse = await CommonUtils.ApiResponseProvider.GetApiData<StudentsNS.Students>(ApiEndPoints.CsvUsersStudents) as StudentsNS.Students;
            var studentsResponseInfo = (from s in studentsResponse.Property1 
                                        let mainTelephone = (s.telephones == null || s.telephones.Count() == 0) ? null : s.telephones.FirstOrDefault()
                                        let mainTelephoneNumber = mainTelephone == null ? "" : (string)mainTelephone.telephoneNumber
                                        let emailAddress = (s.electronicMails == null || s.electronicMails.Count() == 0) ? "" : (string)s.electronicMails[0].electronicMailAddress //TODO: just pick 0?.  or get based on electronicMailType field.
                                        let mobile = (s.telephones == null || s.telephones.Count() == 0) ? null : s.telephones.FirstOrDefault(x => (string)x.telephoneNumberType == "Mobile")
                                        let mobileNumber = mobile == null ? "" : mobile.telephoneNumber
                                        select new
                                        {
                                            id = s.id,
                                            userId = s.studentUniqueId,
                                            givenName = s.firstName,
                                            familyName = s.lastSurname,
                                            middleName = s.middleName,
                                            identifier = s.studentUniqueId,
                                            email = emailAddress,
                                            sms = mobileNumber,
                                            phone = mainTelephoneNumber,
                                            username = s.loginId,
                                            //grade = s.schoolAssociations.FirstOrDefault().gradeLevel
                                            grade = GetHighestGrade(s.schoolAssociations),//Get highest Grade 
                                            sex = s.sexType,
                                            american = s.races.ToList().Any(race=>race.raceType== "Asian")?"True":"False",// s.races.Contains(new StudentsNS.Race().raceType="Asian")?"Tue":"False",
                                            asian = s.races.ToList().Any(race => race.raceType == "Asian") ? "True" : "False",
                                            blackOrAfricanAmerican = s.races.ToList().Any(race => race.raceType == "Black - African American") ? "True" : "False",
                                            nativeHawaiianOrOtherPacificIslander = "",
                                            white = s.races.ToList().Any(race => race.raceType == "White") ? "True" : "False",
                                            demographicRaceTwoOrMoreRaces = (s.races.ToList().Count>1)?"True":"False",
                                            hispanicOrLatinoEthnicity = s.hispanicLatinoEthnicity,
                                            countryOfBirthCode = s.birthCountryDescriptor,
                                            stateOfBirthAbbreviation = s.birthStateAbbreviationType,
                                            cityOfBirth = s.birthCity,
                                            publicSchoolResidenceStatus = ""
                                        }).ToList();

            var staffResponse = await CommonUtils.ApiResponseProvider.GetApiData<StaffNS.Staffs>(ApiEndPoints.CsvUsersStaff) as StaffNS.Staffs;
            var staffResponseInfo = (from s in staffResponse.Property1
                                     let mainTelephone = (s.telephones == null || s.telephones.Count() == 0) ? null : s.telephones.FirstOrDefault()
                                     let mainTelephoneNumber = mainTelephone == null ? "" : mainTelephone.telephoneNumber
                                     let emailAddress = (s.electronicMails == null || s.electronicMails.Count() == 0) ? "" : s.electronicMails[0].electronicMailAddress //TODO: just pick 0?.  or get based on electronicMailType field.
                                     let mobile = (s.telephones == null || s.telephones.Count() == 0) ? null : s.telephones.FirstOrDefault(x => x.telephoneNumberType == "Mobile")
                                     let mobileNumber = mobile == null ? "" : mobile.telephoneNumber
                                     select new
                                     {
                                         id = s.id,
                                         userId = s.staffUniqueId,
                                         givenName = s.firstName,
                                         familyName = s.lastSurname,
                                         middleName = s.middleName,
                                         identifier = s.staffUniqueId,
                                         email = emailAddress,
                                         sms = mobileNumber,
                                         phone = mainTelephoneNumber,
                                         username = s.loginId,
                                         sex = s.sexType,
                                         american = s.races.ToList().Any(race => race.raceType == "Asian") ? "True" : "False",// s.races.Contains(new StudentsNS.Race().raceType="Asian")?"Tue":"False",
                                         asian = s.races.ToList().Any(race => race.raceType == "Asian") ? "True" : "False",
                                         blackOrAfricanAmerican = s.races.ToList().Any(race => race.raceType == "Black - African American") ? "True" : "False",
                                         nativeHawaiianOrOtherPacificIslander = "",
                                         white = s.races.ToList().Any(race => race.raceType == "White") ? "True" : "False",
                                         demographicRaceTwoOrMoreRaces = (s.races.ToList().Count > 1) ? "True" : "False",
                                         hispanicOrLatinoEthnicity = s.hispanicLatinoEthnicity,
                                         countryOfBirthCode = "",
                                         stateOfBirthAbbreviation = "",
                                         cityOfBirth = "",
                                         publicSchoolResidenceStatus = ""
                                     }).ToList();

            var studentInfo = (from e in studentSchoolAssoctionList
                               from si in studentsResponseInfo.Where(x => x.userId == e.StudetUniqueId)
                               select new CsvDemographics
                               {
                                   sourcedId = si.id,
                                   status="",
                                   sex = si.sex,
                                   americanIndianOrAlaskaNative = si.american,
                                   asian = si.asian,
                                   blackOrAfricanAmerican = si.blackOrAfricanAmerican,
                                   nativeHawaiianOrOtherPacificIslander = si.nativeHawaiianOrOtherPacificIslander,
                                   white = si.white,
                                   demographicRaceTwoOrMoreRaces = si.demographicRaceTwoOrMoreRaces,
                                   hispanicOrLatinoEthnicity = si.hispanicOrLatinoEthnicity,
                                   countryOfBirthCode = si.countryOfBirthCode,
                                   stateOfBirthAbbreviation = si.stateOfBirthAbbreviation,
                                   cityOfBirth = si.cityOfBirth,
                                   publicSchoolResidenceStatus = ""
                               }).ToList();

            var staffInfo = (from e in enrollmentsList
                             from s in e.staffs
                             from si in staffResponseInfo.Where(x => x.id == s.id)
                             select new CsvDemographics
                             {
                                 sourcedId = s.id,
                                 sex = si.sex,
                                 americanIndianOrAlaskaNative = si.american,
                                 asian = si.asian,
                                 blackOrAfricanAmerican = si.blackOrAfricanAmerican,
                                 nativeHawaiianOrOtherPacificIslander = si.nativeHawaiianOrOtherPacificIslander,
                                 white = si.white,
                                 demographicRaceTwoOrMoreRaces = si.demographicRaceTwoOrMoreRaces,
                                 hispanicOrLatinoEthnicity = si.hispanicOrLatinoEthnicity?"True":"False", //
                                 countryOfBirthCode = si.countryOfBirthCode,
                                 stateOfBirthAbbreviation = si.stateOfBirthAbbreviation,
                                 cityOfBirth = si.cityOfBirth,
                                 publicSchoolResidenceStatus = ""
                             }).ToList();

            var distinctStudents = studentInfo.GroupBy(x => new { x.sourcedId }).Select(group => group.First()).ToList();
            var distinctStaff = staffInfo.GroupBy(x => new { x.sourcedId }).Select(group => group.First());
            var studentsAndStaff = distinctStudents.Concat(distinctStaff).ToList();
            return studentsAndStaff.ToList();
        }

        private static async Task<PagedDataResults> GetPagedCourses(FilterInputs inputs, int pageNumber)
        {
            var model = new PagedDataResults();
            if (pageNumber != 0)
            {
                model = (PagedDataResults)CommonUtils.HttpContextProvider.Current.Session["CoursesDataPreviewResults"];
            }
            else
            {
                pageNumber = 1;
                model.Courses = new List<CsvCourses>();
                model.Inputs = inputs;
                CommonUtils.HttpContextProvider.Current.Session["CoursesDataPreviewResults"] = model;
            }

            bool getMore = model.Courses == null || model.Courses.Count() < (pageNumber * _dataPreviewPageSize);
            getMore = getMore && model.TotalPages == 0; //total pages is 0 until we got them all and know the max
            while (getMore)
            {
                var responseArray = await CommonUtils.ApiResponseProvider.GetPagedApiData(ApiEndPoints.CsvCourses, model.CurrentOffset);
                var enrollmentsList = (from o in responseArray
                                       let teachers = o["staff"].Children().Select(x => (string)x["id"])
                                       select new CsvCourses
                                       {
                                           sourcedId = (string)o["courseOfferingReference"]["id"],
                                           schoolYearId = (string)o["sessionReference"]["id"],
                                           title = (string)o["courseOfferingReference"]["localCourseCode"],
                                           courseCode = (string)o["courseOfferingReference"]["localCourseCode"],
                                           orgSourcedId = (string)o["schoolReference"]["id"],
                                           subjects = (string)o["academicSubjectDescriptor"],
                                           SchoolId = (string)o["schoolReference"]["id"],
                                           Section = (string)o["uniqueSectionCode"],
                                           Teachers = teachers
                                       });

                var recordsReturnedFromApi = enrollmentsList.Count();

                if (model.Inputs != null)
                {
                    if (model.Inputs.Schools != null)
                        enrollmentsList = enrollmentsList.Where(x => model.Inputs.Schools.Contains(x.SchoolId));

                    if (model.Inputs.Sections != null)
                        enrollmentsList = enrollmentsList.Where(x => model.Inputs.Sections.Contains(x.Section));

                    if (model.Inputs.Teachers != null)
                        enrollmentsList = enrollmentsList.Where(x => x.Teachers.Intersect(model.Inputs.Teachers).Any());
                }


                model.CurrentOffset += _maxApiCallSize;

                enrollmentsList = enrollmentsList.GroupBy(x => x.sourcedId).Select(group => group.First());
                if (enrollmentsList.Count() > 0)
                {
                    var enrollmentsToAdd = enrollmentsList.Where(p0 => model.Courses.Where(p1 => p1.sourcedId == p0.sourcedId).Count() == 0);
                    model.Courses.AddRange(enrollmentsToAdd);
                    model.Courses = model.Courses.GroupBy(x => x.sourcedId).Select(group => group.First()).ToList();
                }

                if (recordsReturnedFromApi < _maxApiCallSize)
                {
                    getMore = false;
                    model.TotalPages = (model.Courses.Count() + _dataPreviewPageSize - 1) / _dataPreviewPageSize; //http://stackoverflow.com/questions/17944/how-to-round-up-the-result-of-integer-division
                }

                if (model.Courses.Count() >= (pageNumber * _dataPreviewPageSize))
                {
                    getMore = false;
                }
            }

            var startAt = (pageNumber - 1) * _dataPreviewPageSize;
            model.CoursesCurentPage = model.Courses.Skip(startAt).Take(_dataPreviewPageSize).ToList();
            return model;
        }

        private static async Task<List<CsvClasses>> GetCsvClasses(FilterInputs inputs)
        {
            var responseArray = await CommonUtils.ApiResponseProvider.GetApiData<SectionsNS.Sections>(ApiEndPoints.CsvClasses) as SectionsNS.Sections;
            var enrollmentsList = (from o in responseArray.Property1
                                       //let teachers = o.staff.Select(x => x.id)
                                   let staffs = o.staff
                                   let teachers = o.staff.Select(x => x.id)
                                   select new CsvClasses
                                   {
                                       sourcedId = o.id,
                                       title = o.uniqueSectionCode,
                                       courseSourcedId = o.courseOfferingReference.id,
                                       classCode = o.uniqueSectionCode,
                                       classType = "scheduled",
                                       schoolSourcedId = o.schoolReference.id,
                                       termSourcedId = o.sessionReference.id,
                                       subjects = o.academicSubjectDescriptor,
                                       SchoolId = o.schoolReference.id,
                                       SchoolYear = Convert.ToString(o.courseOfferingReference.schoolYear),
                                       //ade =o
                                       //Term = o.courseOfferingReference.termDescriptor,
                                       //Subject = o.academicSubjectDescriptor,
                                       //Course = o.courseOfferingReference.localCourseCode,
                                       Section = o.uniqueSectionCode,
                                       Teachers = teachers
                                   }).ToList();
            if (inputs != null)
            {
                if (inputs.Schools != null)
                    enrollmentsList = enrollmentsList.Where(x => inputs.Schools.Contains(x.SchoolId)).ToList();

                if (inputs.SchoolYears != null)
                    enrollmentsList = enrollmentsList.Where(x => inputs.SchoolYears.Contains(x.SchoolYear)).ToList();

                //if (inputs.Sections != null)
                //    enrollmentsList = enrollmentsList.Where(x => inputs.Sections.Contains(x.Section));

                //if (inputs.Teachers != null)
                //    enrollmentsList = enrollmentsList.Where(x => x.Teachers.Intersect(inputs.Teachers).Any());
            }

            return enrollmentsList.ToList();
        }

        private static async Task<PagedDataResults> GetPagedClasses(FilterInputs inputs, int pageNumber)
        {
            var model = new PagedDataResults();
            if (pageNumber != 0)
            {
                model = (PagedDataResults)CommonUtils.HttpContextProvider.Current.Session["ClassesDataPreviewResults"];
            }
            else
            {
                pageNumber = 1;
                model.Classes = new List<CsvClasses>();
                model.Inputs = inputs;
                CommonUtils.HttpContextProvider.Current.Session["ClassesDataPreviewResults"] = model;
            }

            bool getMore = model.Classes == null || model.Classes.Count() < (pageNumber * _dataPreviewPageSize);
            getMore = getMore && model.TotalPages == 0; //total pages is 0 until we got them all and know the max
            while (getMore)
            {
                var responseArray = await CommonUtils.ApiResponseProvider.GetPagedApiData(ApiEndPoints.CsvClasses, model.CurrentOffset);
                var enrollmentsList = (from o in responseArray
                                       let teachers = o["staff"].Children().Select(x => (string)x["id"])
                                       select new CsvClasses
                                       {
                                           sourcedId = (string)o["id"],
                                           title = (string)o["uniqueSectionCode"],
                                           courseSourcedId = (string)o["courseOfferingReference"]["id"],
                                           classCode = (string)o["uniqueSectionCode"],
                                           classType = "scheduled",
                                           schoolSourcedId = (string)o["schoolReference"]["id"],
                                           termSourcedId = (string)o["sessionReference"]["id"],
                                           subjects = (string)o["academicSubjectDescriptor"],
                                           SchoolId = (string)o["schoolReference"]["id"],
                                           Section = (string)o["uniqueSectionCode"],
                                           Teachers = teachers
                                       });

                var recordsReturnedFromApi = enrollmentsList.Count();

                if (model.Inputs != null)
                {
                    if (model.Inputs.Schools != null)
                        enrollmentsList = enrollmentsList.Where(x => model.Inputs.Schools.Contains(x.SchoolId));

                    if (model.Inputs.Sections != null)
                        enrollmentsList = enrollmentsList.Where(x => model.Inputs.Sections.Contains(x.Section));

                    if (model.Inputs.Teachers != null)
                        enrollmentsList = enrollmentsList.Where(x => x.Teachers.Intersect(model.Inputs.Teachers).Any());
                }

                model.CurrentOffset += _maxApiCallSize;

                enrollmentsList = enrollmentsList.GroupBy(x => x.sourcedId).Select(group => group.First());
                model.Classes.AddRange(enrollmentsList);
                model.Classes = model.Classes.GroupBy(x => x.sourcedId).Select(group => group.First()).ToList();

                if (recordsReturnedFromApi < _maxApiCallSize)
                {
                    getMore = false;
                    model.TotalPages = (model.Classes.Count() + _dataPreviewPageSize - 1) / _dataPreviewPageSize; //http://stackoverflow.com/questions/17944/how-to-round-up-the-result-of-integer-division
                }

                if (model.Classes.Count() >= (pageNumber * _dataPreviewPageSize))
                {
                    getMore = false;
                }
            }

            var startAt = (pageNumber - 1) * _dataPreviewPageSize;
            model.ClassesCurentPage = model.Classes.Skip(startAt).Take(_dataPreviewPageSize).ToList();
            return model;
        }

        private static async Task<List<CsvEnrollments>> GetCsvEnrollments(FilterInputs inputs)
        {
            var responseArray = await CommonUtils.ApiResponseProvider.GetApiData<SectionsNS.Sections>(ApiEndPoints.CsvEnrollments) as SectionsNS.Sections;
            var enrollmentsList = (from o in responseArray.Property1
                                   let students = o.students
                                   let staffs = o.staff
                                   let teachers = o.staff.Select(x => x.id)
                                   select new
                                   {
                                       classSourcedId = o.id,
                                       schoolSourcedId = o.schoolReference.id,
                                       students = students,
                                       staffs = staffs,
                                       SchoolId = o.schoolReference.id,
                                       SchoolYear = Convert.ToString(o.courseOfferingReference.schoolYear),
                                       //Term = o.courseOfferingReference.termDescriptor,
                                       //Subject = o.academicSubjectDescriptor,
                                       //Course = o.courseOfferingReference.localCourseCode,
                                       Section = o.uniqueSectionCode,
                                       Teachers = teachers
                                   }).ToList();

            if (inputs != null)
            {
                if (inputs.Schools != null)
                    enrollmentsList = enrollmentsList.Where(x => inputs.Schools.Contains(x.SchoolId)).ToList();

                if (inputs.SchoolYears != null)
                    enrollmentsList = enrollmentsList.Where(x => inputs.SchoolYears.Contains(x.SchoolYear)).ToList();

                //if (inputs.Terms != null)
                //    enrollmentsList = enrollmentsList.Where(x => inputs.Terms.Contains(x.Term));

                //if (inputs.Subjects != null)
                //    enrollmentsList = enrollmentsList.Where(x => inputs.Subjects.Contains(x.Subject));

                //if (inputs.Courses != null)
                //    enrollmentsList = enrollmentsList.Where(x => inputs.Courses.Contains(x.Course));

                //if (inputs.Sections != null)
                //    enrollmentsList = enrollmentsList.Where(x => inputs.Sections.Contains(x.Section));

                //if (inputs.Teachers != null)
                //    enrollmentsList = enrollmentsList.Where(x => x.Teachers.Intersect(inputs.Teachers).Any());
            }

            var studentInfo = (from e in enrollmentsList
                               from s in e.students
                               select new CsvEnrollments
                               {
                                   sourcedId = s.studentSectionAssociation_id,
                                   classSourcedId = e.classSourcedId,
                                   schoolSourcedId = e.schoolSourcedId,
                                   userSourcedId = s.id,
                                   role = "student"
                               }).ToList();

            var staffInfo = (from e in enrollmentsList
                             from s in e.staffs
                             select new CsvEnrollments
                             {
                                 sourcedId = (string)s.staffSectionAssociation_id,
                                 classSourcedId = e.classSourcedId,
                                 schoolSourcedId = e.schoolSourcedId,
                                 userSourcedId = s.id,
                                 role = "teacher"
                             }).ToList();

            var allEnrollments = studentInfo.Concat(staffInfo).ToList();

            return allEnrollments;
        }

        private static async Task<PagedDataResults> GetPagedEnrollments(FilterInputs inputs, int pageNumber)
        {
            var model = new PagedDataResults();
            if (pageNumber != 0)
            {
                model = (PagedDataResults)CommonUtils.HttpContextProvider.Current.Session["EnrollmentsDataPreviewResults"];
            }
            else
            {
                pageNumber = 1;
                model.Enrollments = new List<CsvEnrollments>();
                model.Inputs = inputs;
                CommonUtils.HttpContextProvider.Current.Session["EnrollmentsDataPreviewResults"] = model;
            }

            bool getMore = model.Enrollments == null || model.Enrollments.Count() < (pageNumber * _dataPreviewPageSize);
            getMore = getMore && model.TotalPages == 0; //total pages is 0 until we got them all and know the max
            while (getMore)
            {
                var responseArray = await CommonUtils.ApiResponseProvider.GetPagedApiData(ApiEndPoints.CsvEnrollments, model.CurrentOffset);
                var enrollmentsList = (from o in responseArray
                                       let students = o["students"].Children()
                                       let staffs = o["staff"].Children()
                                       let teachers = o["staff"].Children().Select(x => (string)x["id"])
                                       select new
                                       {
                                           classSourcedId = (string)o["id"],
                                           schoolSourcedId = (string)o["schoolReference"]["id"],
                                           students = students,
                                           staffs = staffs,
                                           SchoolId = (string)o["schoolReference"]["id"],
                                           Section = (string)o["uniqueSectionCode"],
                                           Teachers = teachers
                                       });

                var recordsReturnedFromApi = enrollmentsList.Count();

                if (model.Inputs != null)
                {
                    if (model.Inputs.Schools != null)
                        enrollmentsList = enrollmentsList.Where(x => model.Inputs.Schools.Contains(x.SchoolId));

                    if (model.Inputs.Sections != null)
                        enrollmentsList = enrollmentsList.Where(x => model.Inputs.Sections.Contains(x.Section));

                    if (model.Inputs.Teachers != null)
                        enrollmentsList = enrollmentsList.Where(x => x.Teachers.Intersect(model.Inputs.Teachers).Any());
                }

                var studentInfo = (from e in enrollmentsList
                                   from s in e.students
                                   select new CsvEnrollments
                                   {
                                       sourcedId = (string)s["studentSectionAssociation_id"],
                                       classSourcedId = e.classSourcedId,
                                       schoolSourcedId = e.schoolSourcedId,
                                       userSourcedId = (string)s["id"],
                                       role = "student"
                                   });

                var staffInfo = (from e in enrollmentsList
                                 from s in e.staffs
                                 select new CsvEnrollments
                                 {
                                     sourcedId = (string)s["staffSectionAssociation_id"],
                                     classSourcedId = e.classSourcedId,
                                     schoolSourcedId = e.schoolSourcedId,
                                     userSourcedId = (string)s["id"],
                                     role = "teacher"
                                 });

                var concatenatedEnrollments = studentInfo.Concat(staffInfo).ToList();

                model.CurrentOffset += _maxApiCallSize;

                model.Enrollments.AddRange(concatenatedEnrollments);

                if (recordsReturnedFromApi < _maxApiCallSize)
                {
                    getMore = false;
                    model.TotalPages = (model.Enrollments.Count() + _dataPreviewPageSize - 1) / _dataPreviewPageSize; //http://stackoverflow.com/questions/17944/how-to-round-up-the-result-of-integer-division
                }

                if (model.Enrollments.Count() >= (pageNumber * _dataPreviewPageSize))
                {
                    getMore = false;
                }
            }

            var startAt = (pageNumber - 1) * _dataPreviewPageSize;
            model.EnrollmentsCurentPage = model.Enrollments.Skip(startAt).Take(_dataPreviewPageSize).ToList();
            return model;
        }

        private static async Task<List<CsvAcademicSessions>> GetCsvAcademicSessions(FilterInputs inputs)
        {
            var responseArray = await CommonUtils.ApiResponseProvider.GetApiData<SectionsNS.Sections>(ApiEndPoints.CsvAcademicSessions) as SectionsNS.Sections;

            var context = new ApplicationDbContext();
            var typeDictionary = context.AcademicSessionTypes.ToDictionary(t => t.TermDescriptor, t => t.Type);

            var enrollmentsList = (from o in responseArray.Property1
                                   let teachers = o.staff.Select(x => x.id)
                                   let termDescriptor = o.sessionReference.termDescriptor
                                   let type = typeDictionary.ContainsKey(termDescriptor) ? typeDictionary[termDescriptor] : ""
                                   let startDate = DateTime.Parse(o.sessionReference.beginDate)
                                   let endDate = DateTime.Parse(o.sessionReference.endDate)
                                   select new CsvAcademicSessions
                                   {
                                       sourcedId = o.sessionReference.id,
                                       title = o.sessionReference.schoolYear + " " + termDescriptor,
                                       type = type,
                                       startDate = startDate.ToString("yyyy-MM-dd"),
                                       endDate = endDate.ToString("yyyy-MM-dd"),
                                       SchoolId = o.schoolReference.id,
                                       SchoolYear = Convert.ToString(o.courseOfferingReference.schoolYear),
                                       //Term = o.courseOfferingReference.termDescriptor,
                                       //Subject = o.academicSubjectDescriptor,
                                       //Course = o.courseOfferingReference.localCourseCode,
                                       Section = o.uniqueSectionCode,
                                       Teachers = teachers,
                                       schoolYear = o.sessionReference.schoolYear
                                   }).ToList();

            if (inputs != null)
            {
                if (inputs.Schools != null)
                    enrollmentsList = enrollmentsList.Where(x => inputs.Schools.Contains(x.SchoolId)).ToList();

                if (inputs.SchoolYears != null)
                    enrollmentsList = enrollmentsList.Where(x => inputs.SchoolYears.Contains(x.SchoolYear)).ToList();

                //if (inputs.Terms != null)
                //    enrollmentsList = enrollmentsList.Where(x => inputs.Terms.Contains(x.Term));

                //if (inputs.Subjects != null)
                //    enrollmentsList = enrollmentsList.Where(x => inputs.Subjects.Contains(x.Subject));

                //if (inputs.Courses != null)
                //    enrollmentsList = enrollmentsList.Where(x => inputs.Courses.Contains(x.Course));

                //if (inputs.Sections != null)
                //    enrollmentsList = enrollmentsList.Where(x => inputs.Sections.Contains(x.Section));

                //if (inputs.Teachers != null)
                //    enrollmentsList = enrollmentsList.Where(x => x.Teachers.Intersect(inputs.Teachers).Any());
            }

            enrollmentsList = enrollmentsList.GroupBy(x => x.sourcedId).Select(group => group.First()).ToList();

            context.Dispose();
            return enrollmentsList.ToList();
        }

        private static async Task<UsersPagedDataResults> GetPagedUsers(FilterInputs inputs, int pageNumber)
        {
            var model = new UsersPagedDataResults();
            if (pageNumber != 0)
            {
                model = (UsersPagedDataResults)CommonUtils.HttpContextProvider.Current.Session["UsersDataPreviewResults"];
            }
            else
            {
                pageNumber = 1;
                model.Users = new List<CsvUsers>();
                model.Inputs = inputs;
                CommonUtils.HttpContextProvider.Current.Session["UsersDataPreviewResults"] = model;
            }
            bool getMore = model.Users == null || model.Users.Count() < (pageNumber * _dataPreviewPageSize);
            getMore = getMore && model.TotalPages == 0; //total pages is 0 until we got them all and know the max
            List<CsvUsers> lstCSVUsers = new List<CsvUsers>();
            SectionsNS.Sections enrollmentsResponse = CommonUtils.HttpContextProvider.Current.Session["PagedUsersEnrollments"] as SectionsNS.Sections;
            if (enrollmentsResponse == null)
                enrollmentsResponse = await CommonUtils.ApiResponseProvider.GetApiData<SectionsNS.Sections>(ApiEndPoints.CsvUsers) as SectionsNS.Sections;
            CommonUtils.HttpContextProvider.Current.Session["PagedUsersEnrollments"] = enrollmentsResponse;
            var enrollmentsList = (from o in enrollmentsResponse.Property1
                                   let students = o.students.Select(x => x.id)
                                   let staffs = o.staff.Select(x => x)
                                   let teachers = o.staff.Select(x => x.id)
                                   select new
                                   {
                                       students = students,
                                       staffs = staffs,
                                       SchoolId = o.schoolReference.id,
                                       //SchoolYear = Convert.ToString(o.courseOfferingReference.schoolYear),
                                       //Term = o.courseOfferingReference.termDescriptor,
                                       //Subject = o.academicSubjectDescriptor,
                                       //Course = o.courseOfferingReference.localCourseCode,
                                       Section = o.uniqueSectionCode,
                                       Teachers = teachers
                                   });
            if (model.Inputs != null)
            {
                if (model.Inputs.Schools != null)
                    enrollmentsList = enrollmentsList.Where(x => model.Inputs.Schools.Contains(x.SchoolId));

                if (model.Inputs.Sections != null)
                    enrollmentsList = enrollmentsList.Where(x => model.Inputs.Sections.Contains(x.Section));

                if (model.Inputs.Teachers != null)
                    enrollmentsList = enrollmentsList.Where(x => x.Teachers.Intersect(model.Inputs.Teachers).Any());
            }
            enrollmentsList = enrollmentsList.ToList();
            List<KeyValuePair<int, List<StudentsNS.Class1>>> studentsPages = new List<KeyValuePair<int, List<StudentsNS.Class1>>>();
            List<KeyValuePair<int, List<StaffNS.Class1>>> staffPages = new List<KeyValuePair<int, List<StaffNS.Class1>>>();
            //bool calculateTotalPages = false;
            bool allStudentsPagesProcessed = false;
            bool allTeachersPagesProcessed = false;
            while (getMore)
            {
                try
                {
                    bool lastStudentsPageForCurrentEnrollmentPage = false;
                    bool lastStaffPageForCurrentEnrollmentPage = false;
                    bool getMoreStudents = true;
                    while (getMoreStudents)
                    {
                        List<StudentsNS.Class1> studentsResponse = studentsPages.Where(p => p.Key == model.StudentsPagedDataResults.CurrentOffset).Select(p => p.Value).FirstOrDefault();
                        if (studentsResponse == null)
                        {
                            var studentsResponseArray = await CommonUtils.ApiResponseProvider.GetPagedApiData(ApiEndPoints.CsvUsersStudents, model.StudentsPagedDataResults.CurrentOffset);
                            studentsResponse = studentsResponseArray.ToObject<List<StudentsNS.Class1>>();
                            studentsPages.Add(new KeyValuePair<int, List<StudentsNS.Class1>>(model.CurrentOffset, studentsResponse));
                        }
                        if (studentsResponse.Count == 0)
                        {
                            getMoreStudents = false;
                            lastStudentsPageForCurrentEnrollmentPage = true;
                            continue;
                        }
                        var studentsResponseInfo = (from s in studentsResponse
                                                    let mainTelephone = (s.telephones == null || s.telephones.Count() == 0) ? null : s.telephones.FirstOrDefault(x => x.orderOfPriority == "1")
                                                    let mainTelephoneNumber = mainTelephone == null ? "" : (string)mainTelephone.telephoneNumber
                                                    let emailAddress = (s.electronicMails == null || s.electronicMails.Count() == 0) ? "" : (string)s.electronicMails[0].electronicMailAddress //TODO: just pick 0?.  or get based on electronicMailType field.
                                                    let mobile = (s.telephones == null || s.telephones.Count() == 0) ? null : s.telephones.FirstOrDefault(x => (string)x.telephoneNumberType == "Mobile")
                                                    let mobileNumber = mobile == null ? "" : mobile.telephoneNumber
                                                    select new
                                                    {
                                                        id = s.id,
                                                        userId = s.studentUniqueId,
                                                        givenName = s.firstName,
                                                        familyName = s.lastSurname,
                                                        middleName = s.middleName,
                                                        identifier = s.studentUniqueId,
                                                        email = emailAddress,
                                                        sms = mobileNumber,
                                                        phone = mainTelephoneNumber,
                                                        username = s.loginId
                                                    }).ToList();

                        var studentInfo = (from e in enrollmentsList
                                           from s in e.students
                                           from si in studentsResponseInfo.Where(x => x.id == s)
                                           select new CsvUsers
                                           {
                                               sourcedId = s,
                                               orgSourcedIds = e.SchoolId,
                                               enabledUser = "TRUE",
                                               role = "student",
                                               userId = si.userId,
                                               givenName = si.givenName,
                                               familyName = si.familyName,
                                               middleNames = si.middleName,
                                               identifier = si.identifier,
                                               email = si.email,
                                               sms = si.sms,
                                               phone = si.phone,
                                               username = si.username
                                           }).ToList();
                        var distinctStudents = studentInfo.GroupBy(x => new { x.sourcedId, x.SchoolId }).Select(group => group.First()).ToList();
                        if (distinctStudents.Count > 0)
                        {
                            if (model.Users == null)
                            {
                                model.Users = new List<CsvUsers>();
                            }
                            var studentsToAdd = distinctStudents.Where(p0 => model.Users.Where(p1 => p1.sourcedId == p0.sourcedId).Count() == 0);
                            if (studentsToAdd.Count() > 0)
                            {
                                lstCSVUsers.AddRange(studentsToAdd);
                                model.Users.AddRange(studentsToAdd);
                            }
                        }
                        if (studentsResponse.Count() < _maxApiCallSize)
                        {
                            getMoreStudents = false;
                            lastStudentsPageForCurrentEnrollmentPage = true;
                        }
                        if (lstCSVUsers.Count >= _maxApiCallSize)
                            getMoreStudents = false;
                        if (getMoreStudents || distinctStudents.Count > 0)
                            model.StudentsPagedDataResults.CurrentOffset += _maxApiCallSize;
                    }
                    if (lastStudentsPageForCurrentEnrollmentPage)
                        allStudentsPagesProcessed = true;
                    if (lstCSVUsers.Count < _maxApiCallSize)
                    {
                        bool getMoreStaff = true;
                        while (getMoreStaff)
                        {
                            List<StaffNS.Class1> staffResponse = staffPages.Where(p => p.Key == model.StaffPagedDataResults.CurrentOffset).Select(p => p.Value).FirstOrDefault();
                            if (staffResponse == null)
                            {
                                //end of students. We can now start processing teachers
                                var staffResponseArray = await CommonUtils.ApiResponseProvider.GetPagedApiData(ApiEndPoints.CsvUsersStaff, model.StaffPagedDataResults.CurrentOffset);
                                staffResponse = staffResponseArray.ToObject<List<StaffNS.Class1>>();
                            }
                            if (staffResponse.Count == 0)
                            {
                                getMoreStaff = false;
                                lastStaffPageForCurrentEnrollmentPage = true;
                                continue;
                            }
                            var staffResponseInfo = (from s in staffResponse
                                                     let mainTelephone = (s.telephones == null || s.telephones.Count() == 0) ? null : s.telephones.FirstOrDefault(x => x.orderOfPriority == "1")
                                                     let mainTelephoneNumber = mainTelephone == null ? "" : mainTelephone.telephoneNumber
                                                     let emailAddress = (s.electronicMails == null || s.electronicMails.Count() == 0) ? "" : s.electronicMails[0].electronicMailAddress //TODO: just pick 0?.  or get based on electronicMailType field.
                                                     let mobile = (s.telephones == null || s.telephones.Count() == 0) ? null : s.telephones.FirstOrDefault(x => x.telephoneNumberType == "Mobile")
                                                     let mobileNumber = mobile == null ? "" : mobile.telephoneNumber
                                                     select new
                                                     {
                                                         id = s.id,
                                                         userId = s.staffUniqueId,
                                                         givenName = s.firstName,
                                                         familyName = s.lastSurname,
                                                         middleName = s.middleName,
                                                         identifier = s.staffUniqueId,
                                                         email = emailAddress,
                                                         sms = mobileNumber,
                                                         phone = mainTelephoneNumber,
                                                         username = s.loginId
                                                     }).ToList();
                            var staffInfo = (from e in enrollmentsList
                                             from s in e.staffs
                                             from si in staffResponseInfo.Where(x => x.id == s.id)
                                             select new CsvUsers
                                             {
                                                 sourcedId = s.id,
                                                 orgSourcedIds = e.SchoolId,
                                                 enabledUser = "TRUE",
                                                 role = "teacher",
                                                 userId = si.userId,
                                                 givenName = si.givenName,
                                                 familyName = si.familyName,
                                                 middleNames = si.middleName,
                                                 identifier = si.identifier,
                                                 email = si.email,
                                                 sms = si.sms,
                                                 phone = si.phone,
                                                 username = si.username
                                             }).ToList();
                            var distinctStaff = staffInfo.GroupBy(x => new { x.sourcedId, x.SchoolId }).Select(group => group.First()).ToList();
                            if (distinctStaff.Count > 0)
                            {
                                if (model.Users == null)
                                {
                                    model.Users = new List<CsvUsers>();
                                }
                                var staffToAdd = distinctStaff.Where(p0 => model.Users.Where(p1 => p1.sourcedId == p0.sourcedId).Count() == 0);
                                if (staffToAdd.Count() > 0)
                                {
                                    lstCSVUsers.AddRange(staffToAdd);
                                    model.Users.AddRange(staffToAdd);
                                }
                            }
                            if (staffResponse.Count() < _maxApiCallSize)
                            {
                                getMoreStaff = false;
                                lastStaffPageForCurrentEnrollmentPage = true;
                            }
                            if (lstCSVUsers.Count >= _maxApiCallSize)
                                getMoreStaff = false;
                            if (getMoreStaff && distinctStaff.Count > 0)
                                model.StaffPagedDataResults.CurrentOffset += _maxApiCallSize;
                        }
                        if (lastStaffPageForCurrentEnrollmentPage)
                            allTeachersPagesProcessed = true;
                    }
                    if (lstCSVUsers.Count >= _dataPreviewPageSize)
                        getMore = false;
                    //var sectionsResponseArray = a

                }
                catch (Exception ex)
                {

                }
            }
            if (model.Users != null && model.Users.Count < 0)
            {
                model.Users = model.Users.Distinct().ToList();
            }
            if (allStudentsPagesProcessed && allTeachersPagesProcessed)
            {
                model.TotalPages = (model.Users.Count() + _dataPreviewPageSize - 1) / _dataPreviewPageSize; //http://stackoverflow.com/questions/17944/how-to-round-up-the-result-of-integer-division
                //try
                //{
                //    string path = CommonUtils.PathProvider.MapPath("~/usersPreview.xml");
                //    using (System.IO.StreamWriter writer = new System.IO.StreamWriter(path, false))
                //    {
                //        System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(model.Users.GetType());
                //        serializer.Serialize(writer, model.Users);
                //        writer.Close();
                //    }
                //}
                //catch ( Exception ex)
                //{

                //}
            }
            var startAt = (pageNumber - 1) * _dataPreviewPageSize;
            model.UsersCurentPage = model.Users.Skip(startAt).Take(_dataPreviewPageSize).ToList();
            return model;
        }

        private static async Task<PagedDataResults> GetPagedAcademicSessions(FilterInputs inputs, int pageNumber)
        {
            var model = new PagedDataResults();
            if (pageNumber != 0)
            {
                model = (PagedDataResults)CommonUtils.HttpContextProvider.Current.Session["AcademicSessionsDataPreviewResults"];
            }
            else
            {
                pageNumber = 1;
                model.AcademicSessions = new List<CsvAcademicSessions>();
                model.Inputs = inputs;
                CommonUtils.HttpContextProvider.Current.Session["AcademicSessionsDataPreviewResults"] = model;
            }

            bool getMore = model.AcademicSessions == null || model.AcademicSessions.Count() < (pageNumber * _dataPreviewPageSize);
            getMore = getMore && model.TotalPages == 0; //total pages is 0 until we got them all and know the max
            while (getMore)
            {
                var responseArray = await CommonUtils.ApiResponseProvider.GetPagedApiData(ApiEndPoints.CsvAcademicSessions, model.CurrentOffset);

                var context = new ApplicationDbContext();
                var typeDictionary = context.AcademicSessionTypes.ToDictionary(t => t.TermDescriptor, t => t.Type);

                var enrollmentsList = (from o in responseArray
                                       let teachers = o["staff"].Children().Select(x => (string)x["id"])
                                       let termDescriptor = (string)o["sessionReference"]["termDescriptor"]
                                       let type = typeDictionary.ContainsKey(termDescriptor) ? typeDictionary[termDescriptor] : ""
                                       let startDate = (DateTime)o["sessionReference"]["beginDate"]
                                       let endDate = (DateTime)o["sessionReference"]["endDate"]
                                       select new CsvAcademicSessions
                                       {
                                           sourcedId = (string)o["sessionReference"]["id"],
                                           title = (string)o["sessionReference"]["schoolYear"] + " " + termDescriptor,
                                           type = type,
                                           startDate = startDate.ToString("yyyy-MM-dd"),
                                           endDate = endDate.ToString("yyyy-MM-dd"),
                                           SchoolId = (string)o["schoolReference"]["id"],
                                           SchoolYear = (string)o["courseOfferingReference"]["schoolYear"],
                                           Term = (string)o["courseOfferingReference"]["termDescriptor"],
                                           Subject = (string)o["academicSubjectDescriptor"],
                                           Course = (string)o["courseOfferingReference"]["localCourseCode"],
                                           Section = (string)o["uniqueSectionCode"],
                                           Teachers = teachers,
                                           schoolYear = (string)o["sessionReference"]["schoolYear"]
                                       });

                var recordsReturnedFromApi = enrollmentsList.Count();
                model.CurrentOffset += _maxApiCallSize;
                FilterEnrollments(model.Inputs, ref enrollmentsList);
                enrollmentsList = enrollmentsList.GroupBy(x => x.sourcedId).Select(group => group.First());//do we need to get all, then filter, then group by, then count?  I think so.  THe model mightneed another property for ALL academic session results...like how i did for checkboxes
                var enrollmentsToAdd = enrollmentsList.Where(p0 => model.AcademicSessions.Where(p1 => p1.sourcedId == p0.sourcedId).Count() == 0);
                if (enrollmentsList.Count() > 0)
                    model.AcademicSessions.AddRange(enrollmentsToAdd);

                context.Dispose();

                if (recordsReturnedFromApi < _maxApiCallSize)
                {
                    getMore = false;
                    model.TotalPages = (model.AcademicSessions.Count() + _dataPreviewPageSize - 1) / _dataPreviewPageSize; //http://stackoverflow.com/questions/17944/how-to-round-up-the-result-of-integer-division
                }

                if (model.AcademicSessions.Count() >= (pageNumber * _dataPreviewPageSize))
                {
                    getMore = false;
                }
            }

            var startAt = (pageNumber - 1) * _dataPreviewPageSize;
            model.AcademicSessionsCurrentPage = model.AcademicSessions.Skip(startAt).Take(_dataPreviewPageSize).ToList();
            return model;
        }

        private static void FilterEnrollments(FilterInputs inputs, ref IEnumerable<CsvAcademicSessions> enrollmentsList)
        {
            if (inputs != null)
            {
                if (inputs.Schools != null)
                    enrollmentsList = enrollmentsList.Where(x => inputs.Schools.Contains(x.SchoolId));

                if (inputs.Sections != null)
                    enrollmentsList = enrollmentsList.Where(x => inputs.Sections.Contains(x.Section));

                if (inputs.Teachers != null)
                    enrollmentsList = enrollmentsList.Where(x => x.Teachers.Intersect(inputs.Teachers).Any());
            }
        }

        private static List<CsvManifest> GetCsvManifest(string bulkOrDelta)
        {
            return new List<CsvManifest>
            {
                new CsvManifest { propertyName = "manifest.version", value = "1.0" },
                new CsvManifest { propertyName = "oneroster.version", value = "1.1" },
                new CsvManifest { propertyName = "file.academicSessions", value = bulkOrDelta },
                new CsvManifest { propertyName = "file.categories", value = "absent" },
                new CsvManifest { propertyName = "file.classes", value = bulkOrDelta },
                new CsvManifest { propertyName = "file.classResources", value = "absent" },
                new CsvManifest { propertyName = "file.courses", value = bulkOrDelta },
                new CsvManifest { propertyName = "file.courseResources", value = "absent" },
                new CsvManifest { propertyName = "file.demographics", value = "absent" },
                new CsvManifest { propertyName = "file.enrollments", value = bulkOrDelta },
                new CsvManifest { propertyName = "file.lineItems", value = "absent" },
                new CsvManifest { propertyName = "file.orgs", value = bulkOrDelta },
                new CsvManifest { propertyName = "file.resources", value = "absent" },
                new CsvManifest { propertyName = "file.results", value = "absent" },
                new CsvManifest { propertyName = "file.users", value = bulkOrDelta },
                new CsvManifest { propertyName = "source.systemName", value = "absent" },
                new CsvManifest { propertyName = "source.systemCode", value = "absent" }
            };
        }
        /// <summary>
        /// Get Highest Gade and return
        /// </summary>
        /// <param name="s"> School Asssocations for a Student</param>
        /// <returns></returns>
        private static string GetHighestGrade(StudentsNS.Schoolassociation[] s)
        {
            List<int> lstGrade = new List<int>();
            string gradeLevel = string.Empty;
            IDictionary<int, string> dict = new Dictionary<int, string>();
            try
            {
                dict.Add(-3, "Infant/toddler");
                dict.Add(-2, "Preschool/Prekindergarten");
                dict.Add(-1, "Kindergarten");
                dict.Add(1, "First grade");
                dict.Add(2, "Second grade");
                dict.Add(3, "Third grade");
                dict.Add(4, "Fourth grade");
                dict.Add(5, "Fifth grade");
                dict.Add(6, "Sixth grade");
                dict.Add(7, "Seventh grade");
                dict.Add(8, "Eighth grade");
                dict.Add(9, "Ninth grade");
                dict.Add(10, "Tenth grade");
                dict.Add(11, "Eleventh grade");
                dict.Add(12, "Twelfth grade");
                foreach (StudentsNS.Schoolassociation element in s)
                {
                    if (string.IsNullOrEmpty(element.gradeLevel.Trim()))
                        continue;
                    int key = dict.Where(kvp => kvp.Value == element.gradeLevel).Select(kvp => kvp.Key).FirstOrDefault();
                    lstGrade.Add(key);
                }
                if (lstGrade.Count > 0)
                    gradeLevel = dict[lstGrade.Max()].ToString();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return gradeLevel;
        }
        #endregion

        #region PrivateMethods

        #endregion
    }
}