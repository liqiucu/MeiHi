using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MeiHi.Model;
using MeiHi.Admin.Models;
using PagedList;
using MeiHi.Admin.Models.UserComments;
using MeiHi.Admin.Models.User;
using MeiHi.Admin.Models.Booking;
using MeiHi.Admin.Models.UserSuggests;
using MeiHi.Admin.Models.Statistics;

namespace MeiHi.Admin.Logic
{
    public static class StatisticsLogic
    {
        public static StatisticsUserRegistersModel Statistics(
            DateTime? start = null,
            DateTime? end = null)
        {
            using (var db = new MeiHiEntities())
            {
                StatisticsUserRegistersModel model = new StatisticsUserRegistersModel();
                model.StatisticsTypes = new List<StatisticsTypeModel>();

                if (start != null && end != null)
                {
                    model.TotalRegisterCount = db.User.Count(a => a.DateCreated >= start && a.DateCreated <= end && !string.IsNullOrEmpty(a.FullName));
                    model.TotalDownloadCount = db.User.Count(a => a.DateCreated >= start && a.DateCreated <= end);

                    model.StatisticsTypes.Add(new StatisticsTypeModel()
                    {
                        Type = "ios",
                        RegisterCount = db.User.Count(a => a.Device.ToLower() == "ios" && a.DateCreated >= start && a.DateCreated <= end && !string.IsNullOrEmpty(a.FullName)),
                        DownloadCount = db.User.Count(a => a.Device.ToLower() == "ios" && a.DateCreated >= start && a.DateCreated <= end)
                    });

                    model.StatisticsTypes.Add(new StatisticsTypeModel()
                    {
                        Type = "android",
                        RegisterCount = db.User.Count(a => a.Device.ToLower() == "android" && a.DateCreated >= start && a.DateCreated <= end && !string.IsNullOrEmpty(a.FullName)),
                        DownloadCount = db.User.Count(a => a.Device.ToLower() == "android" && a.DateCreated >= start && a.DateCreated <= end)
                    });

                    model.StatisticsTypes.Add(new StatisticsTypeModel()
                    {
                        Type = "未知",
                        RegisterCount = db.User.Count(a => a.Device.ToLower() != "android" && a.Device.ToLower() != "ios" && a.DateCreated >= start && a.DateCreated <= end && !string.IsNullOrEmpty(a.FullName)),
                        DownloadCount = db.User.Count(a => a.Device.ToLower() != "android" && a.Device.ToLower() != "ios" && a.DateCreated >= start && a.DateCreated <= end)
                    });
                }
                else
                {
                    model.TotalRegisterCount = db.User.Count(a => !string.IsNullOrEmpty(a.FullName));
                    model.TotalDownloadCount = db.User.Count();

                    model.StatisticsTypes.Add(new StatisticsTypeModel()
                    {
                        Type = "ios",
                        RegisterCount = db.User.Count(a => a.Device.ToLower() == "ios" && !string.IsNullOrEmpty(a.FullName)),
                        DownloadCount = db.User.Count(a => a.Device.ToLower() == "ios")
                    });

                    model.StatisticsTypes.Add(new StatisticsTypeModel()
                    {
                        Type = "android",
                        RegisterCount = db.User.Count(a => a.Device.ToLower() == "android" && !string.IsNullOrEmpty(a.FullName)),
                        DownloadCount = db.User.Count(a => a.Device.ToLower() == "android")
                    });

                    model.StatisticsTypes.Add(new StatisticsTypeModel()
                    {
                        Type = "未知",
                        RegisterCount = db.User.Count(a => a.Device.ToLower() != "android" && a.Device.ToLower() != "ios" && !string.IsNullOrEmpty(a.FullName)),
                        DownloadCount = db.User.Count(a => a.Device.ToLower() != "android" && a.Device.ToLower() != "ios")
                    });
                }

                model.TodayRegisterCount = db.User.Count(a => !string.IsNullOrEmpty(a.FullName) && a.DateCreated >= DateTime.Today);
                model.TodayDownloadCount = db.User.Count(a => a.DateCreated >= DateTime.Today);
                return model;
            }
        }
    }
}