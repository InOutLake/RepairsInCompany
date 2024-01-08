using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using RepairsInCompany.Model;
using RepairsInCompany.Model.ViewModel;

namespace RepairsInCompany.Controllers
{
	public class MainController : Controller
	{
		[HttpGet]
		[Authorize]
		public ActionResult ScheduleMonthGraph()
		{
			DateTime observableMonthDate;
			if (ViewBag.ObservableMonthDate == null)
			{
				observableMonthDate = DateTime.Now;
			}
			else
			{
				observableMonthDate = ViewBag.ObservableMonthDate;
			}

			Dictionary<string, List<int>> repairsForNames = new Dictionary<string, List<int>>();

			using (var db = new RepairsDbContext())
			{
				var monthRepairs = db.Repairs.Where(x => x.StartDateTime.Year.Equals(observableMonthDate.Year) &&
													x.StartDateTime.Month.Equals(observableMonthDate.Month)).ToList();
				var equipment = db.Equipment.ToList();

				foreach (var repair in monthRepairs)
				{
					string repairEquipmentName = equipment.Where(x => x.EquipmentId == repair.EquipmentId)
															   .FirstOrDefault().Name;
					if (repairsForNames.Keys.Contains(repairEquipmentName))
						repairsForNames[repairEquipmentName].Add(repair.StartDateTime.Day);
					else
						repairsForNames.Add(repairEquipmentName, new List<int>() { repair.StartDateTime.Day });
				}
			}
			ViewBag.observableMonthDate = observableMonthDate;
			return View(repairsForNames);
		}

		[HttpPost, ActionName("ScheduleMonthGraph")]
		[Authorize]
		public ActionResult ChangeMonthScheduleMonthGraph(DateTime observableMonthDate)
		{
			ViewBag.observableMonthDate = observableMonthDate;
			Dictionary<string, List<int>> repairsForNames = new Dictionary<string, List<int>>();

			using (var db = new RepairsDbContext())
			{
				var monthRepairs = db.Repairs.Where(x => x.StartDateTime.Year.Equals(observableMonthDate.Year) &&
													x.StartDateTime.Month.Equals(observableMonthDate.Month)).ToList();
				var equipment = db.Equipment.ToList();

				foreach (var repair in monthRepairs)
				{
					string repairEquipmentName = equipment.Where(x => x.EquipmentId == repair.EquipmentId)
															   .FirstOrDefault().Name;
					if (repairsForNames.Keys.Contains(repairEquipmentName))
						repairsForNames[repairEquipmentName].Add(repair.StartDateTime.Day);
					else
						repairsForNames.Add(repairEquipmentName, new List<int>() { repair.StartDateTime.Day });
				}
			}
			ViewBag.observableMonthDate = observableMonthDate;
			return View(repairsForNames);
		}

		[HttpGet]
		[Authorize]
		public ActionResult EquipmentList()
		{
			using (var db = new RepairsDbContext())
			{
				var equipmentList = db.Equipment.ToList();
				var registrations = db.Registrations.ToList();
				var equipmentInRepairList = db.EquipmentInRepairs.ToList();
				List<EquipmentListVM> equipmentListVm = new List<EquipmentListVM>();
				foreach (Equipment e in equipmentList)
				{
					bool wasBroken = false;
					bool isInRepair = false;
					if (db.EquipmentBreakDownDates.Where(x => x.Name == e.Name) != null)
						wasBroken = true;
					Registration? registration = registrations.Where(x => x.EquipmentId == e.EquipmentId && x.EndDate == null).FirstOrDefault();
					DateTime? startDateTime = registration != null ? registration.StartDateTime : null;
					isInRepair = !(equipmentInRepairList.Where(x => x.Name == e.Name).FirstOrDefault() == null);
					wasBroken = db.EquipmentBreakDownDates.Where(b => b.Name == e.Name).FirstOrDefault() != null;
					equipmentListVm.Add(new EquipmentListVM() { Name = e.Name, EquipmentId = e.EquipmentId, StartDateTime = startDateTime, WasBroken = wasBroken, IsInRepair = isInRepair });
				}
				equipmentListVm = equipmentListVm.OrderBy(e => e.StartDateTime).ToList();
				return View(equipmentListVm);
			}
		}

		
		[HttpPost, ActionName("MarkBreakdown")]
		[Authorize]
		public ActionResult MarkBreakdown(string selectedEquipmentName)
		{
			if (ModelState.IsValid)
			{
				using (var db = new RepairsDbContext())
				{
					var equipmentNameParam = new SqlParameter("@EquipmentName", selectedEquipmentName);
					int result = db.Database.ExecuteSqlRaw("exec AddRepairUnplanned @EquipmentName={0}", equipmentNameParam);
					return RedirectToAction("OperationResultReturn", "Main", new { message = "Euipment has been successfully added!" });
				}
			}
			ViewBag.Error = "Equipment has not been chosen";
			return Redirect(Request.Headers["Referer"].ToString());
		}

		[HttpGet]
		[Authorize]
		public ActionResult AddEquipment()
		{
			return View();
		}
		[HttpPost]
		public ActionResult AddEquipment(string equipmentName)
		{
			if (ModelState.IsValid)
			{
				using (var db = new RepairsDbContext())
				{
					var equipmentNameParam = new SqlParameter("@EquipmentName", equipmentName);
					int result = -1;
					try
					{
						result = db.Database.ExecuteSqlRaw("exec AddEquipment @EquipmentName={0}", equipmentNameParam);
					}
					catch (Exception e)
					{
						return RedirectToAction("OperationResultReturn", "Main", new { message = "Error occured while adding new equipment! Try another name!" });
					}
					if (result >= 0)
					{
						return RedirectToAction("OperationResult", "Main", new { message = $"Euipment {equipmentName} has been successfully added!" });
					}
					else
					{
						return RedirectToAction("OperationResultReturn", "Main", new { message = "Error occured while adding new equipment! Try another name!" });
					}
				}
			}
			ViewBag.Error = "WrongInput!";
			return View(equipmentName);
		}
		[HttpGet]
		[Authorize]
		public ActionResult OperationResult(string message)
		{
			return View(model: message);
		}

		[HttpGet]
		[Authorize]
		public ActionResult OperationResultReturn(string message)
		{
			return View(model: message);
		}

		[HttpPost]
		[Authorize]
		public ActionResult RepairFork(string equipmentName)
		{
			if (ModelState.IsValid)
			{
				using (var db = new RepairsDbContext())
				{
					if (db.EquipmentInRepairs.Where(e => e.Name == equipmentName).FirstOrDefault() != null)
					{
						return RedirectToAction("RepairConfirmation", "Main", new { equipmentName = equipmentName, confirmed = false });
					}
					else
					{
                        return RedirectToAction("BreakdownConfirmation", "Main", new { equipmentName = equipmentName, confirmed = false });
                    }
				}
			}
			return Redirect(Request.Headers["Referer"].ToString());
		}

		[HttpGet]
		[Authorize]
		public ActionResult RepairConfirmation(string equipmentName, bool confirmed)
		{
			if (ModelState.IsValid)
			{
				if (confirmed)
				{
					using (var db = new RepairsDbContext())
					{
						var equipmentNameParam = new SqlParameter("@EquipmentName", equipmentName);
						int result = db.Database.ExecuteSqlRaw("exec MarkRepairDone @EquipmentName={0}", equipmentNameParam);
						return RedirectToAction("OperationResult", new { message = $"Equipment {equipmentName}'s state now is \"Not in repair\"" });
					}
				}
				else
				{
					return View(model: equipmentName);
				}
			}
			return View(model: equipmentName);
		}

        [HttpGet]
		[Authorize]
		public ActionResult BreakdownConfirmation(string equipmentName, bool confirmed)
        {
            if (ModelState.IsValid)
            {
                if (confirmed)
                {
                    using (var db = new RepairsDbContext())
                    {
                        var equipmentNameParam = new SqlParameter("@EquipmentName", equipmentName);
                        int result = db.Database.ExecuteSqlRaw("exec AddRepairUnplanned @EquipmentName={0}", equipmentNameParam);
                        return RedirectToAction("OperationResult", new { message = $"Equipment {equipmentName}'s state now is \"In repair\"" });
                    }
                }
                else
                {
                    return View(model: equipmentName);
                }
            }
            return View(model: equipmentName);
        }

        [HttpPost]
		[Authorize]
		public ActionResult RegistrationFork(string equipmentName)
        {
            if (ModelState.IsValid)
            {
                using (var db = new RepairsDbContext())
                {
					Guid equipmentId = db.Equipment.Where(e => e.Name == equipmentName).FirstOrDefault().EquipmentId;
                    if (db.Registrations.Where(r => r.EquipmentId == equipmentId && r.EndDate == null).FirstOrDefault() != null)
                    {
                        return RedirectToAction("CloseRegistrationConfirmation", "Main", new { equipmentName = equipmentName, confirmed = false });
                    }
                    else
                    {
                        return RedirectToAction("RegisterEquipment", "Main", new { equipmentName = equipmentName, periodInDays = 0});
                    }
                }
            }
            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpGet]
		[Authorize]
		public ActionResult CloseRegistrationConfirmation(string equipmentName, bool confirmed)
        {
            if (ModelState.IsValid)
            {
                if (confirmed)
                {
                    using (var db = new RepairsDbContext())
                    {
                        var equipmentNameParam = new SqlParameter("@EquipmentName", equipmentName);
                        int result = db.Database.ExecuteSqlRaw("exec CloseRegistration @EquipmentName={0}", equipmentNameParam);
						if (result > 0)
						{
                            return RedirectToAction("OperationResult", new { message = $"Equipment {equipmentName}'s registration has been closed" });
                        }
                        return RedirectToAction("OperationResult", new { message = $"Some error occured. Try again or contact admin" });
                    }
                }
                else
                {
                    return View(model: equipmentName);
                }
            }
            return View(model: equipmentName);
        }

        [HttpGet]
		[Authorize]
		public ActionResult RegisterEquipment(string equipmentName)
        {
            return View(model: Tuple.Create(equipmentName, 0));
        }

        [HttpPost]
        public ActionResult RegisterEquipment(string equipmentName, int periodInDays)
        {
            if (ModelState.IsValid && periodInDays != 0)
            {
                using (var db = new RepairsDbContext())
                {
                    var equipmentNameParam = new SqlParameter("@EquipmentName", equipmentName);
                    var periodInDaysParam = new SqlParameter("@PeriodInDays", periodInDays);
                    int result = db.Database.ExecuteSqlRaw("exec RegisterEquipment @EquipmentName={0}, @PeriodInDays={1}", equipmentNameParam, periodInDaysParam);
                    if (result >= 0)
                    {
                        return RedirectToAction("OperationResult", "Main", new { message = $"Euipment {equipmentName} has been successfully registered with period of {periodInDays} days!" });
                    }
                    else
                    {
                        return RedirectToAction("OperationResultReturn", "Main", new { message = "Error occured while registering equipment! Contact admin or try again later." });
                    }
                }
            }
            Tuple<string, int> model = Tuple.Create(equipmentName, periodInDays);
            return View(Tuple.Create(equipmentName, periodInDays));
        }

		[HttpGet]
		[Authorize]
		public ActionResult UpdateScheduleConfirmation(bool confirmed)
		{
			if (confirmed)
			{
				using (var db = new RepairsDbContext())
				{
                    var yearParam = new SqlParameter("@year", DateTime.Now.Year);
                    int result = db.Database.ExecuteSqlRaw("exec GenerateUpcomingRepairs @year={0}", yearParam);
                    yearParam = new SqlParameter("@year", DateTime.Now.Year + 1);
                    result = db.Database.ExecuteSqlRaw("exec GenerateUpcomingRepairs @year={0}", yearParam);
					return RedirectToAction("OperationResult", "Main", new { message = $"Schedule has been updated." });
                }
            }
			return View();
        }
    }
}
