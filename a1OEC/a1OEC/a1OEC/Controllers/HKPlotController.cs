using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using a1OEC.Models;
using Microsoft.AspNetCore.Http;

namespace a1OEC.Controllers
{
    public class HKPlotController : Controller
    {
        private readonly OECContext _context;

        public HKPlotController(OECContext context)
        {
            _context = context;
        }

        // GET: HKPlot
        public async Task<IActionResult> Index(string name, string cropid, string sort, string where, string varietyId)
        {
            /* if (!String.IsNullOrEmpty(sort))
             {
                 ViewData["SortByFarmName"] = "Farm Name";
                 ViewData["SortByVarietyName"] = "Variety Name";
                 ViewData["SortByCEC"] = "CEC";
             }
             */
            ViewBag.Defalt = String.IsNullOrEmpty(sort) ? "DatePlanted" : "";
            ViewBag.SortByFarmName = sort == "FarmName" ? "FarmDescending" : "Farm";
            ViewBag.SortByVarietyName = sort == "VarietyName" ? "VarietyDescending" : "Variety";
            ViewBag.SortByCEC = sort == "CEC" ? "CECDescending" : "CEC";

            ViewData["VarietyId"] = varietyId;
            ViewData["Cropid"] = cropid;
            ViewBag.where = "Plot page coming from "+ name +"("+where+")";
            ViewBag.forReorderWhere = where;
            ViewBag.forReorderName = name;

            if (!String.IsNullOrEmpty(varietyId))
            {
                var plotForFarm = from p in _context.Plot.Where(p => p.VarietyId == Convert.ToInt16(varietyId)) select p;

                if (plotForFarm.ToList() != null && plotForFarm.ToList().Count() > 0)
                {
                    ViewData["FarmId"] = plotForFarm.ToList()[0].FarmId;
                }
            }

            var plot = from p in _context.Plot.Include(p => p.Farm)
                 .Include(p => p.Variety)
                 .Include(p => p.Treatment)
                 .Include(p => p.Variety.Crop)
                  .Where(p => p.Variety.CropId.ToString() == cropid)
                       select p;
            /// var plot;
            if (!String.IsNullOrEmpty(cropid)) // if the cropid is in the URL or a QueryString variable, save it to a cookie or session variable.
            {
                ViewBag.cropid = cropid;
            }
            
            if (where == "Crop")
            {
               plot = from p in _context.Plot.Include(p => p.Farm)
            .Include(p => p.Variety)
            .Include(p => p.Treatment)
            .Include(p => p.Variety.Crop)
                .Where(p => p.Variety.CropId.ToString() == cropid)
                           select p;
            }
            else if (where == "Variety")
            {
          
              plot = from p in _context.Plot.Include(p => p.Farm)
             .Include(p => p.Variety)
             .Include(p => p.Treatment)
             .Include(p => p.Variety.Crop)
             .Where(p => p.Variety.Name.ToString() == name)
                       select p;
            }

            else
            {
                plot = from p in _context.Plot.Include(p => p.Farm)
           .Include(p => p.Variety)
           .Include(p => p.Treatment)
           .Include(p => p.Variety.Crop)
                       select p;
            }
     
            switch (sort)
            {
                case "Farm":
                    plot = plot.OrderBy(p => p.Farm.Name);
                    break;
                case "FarmDescending":
                    plot = plot.OrderByDescending(p => p.Farm.Name);
                    break;
                case "Variety":
                    plot = plot.OrderBy(p => p.Variety.Name);
                    break;
                case "VarietyDescending":
                    plot = plot.OrderByDescending(p => p.Variety.Name);
                    break;
                case "CEC":
                    plot = plot.OrderBy(p => p.Cec);
                    break;
                case "CECDescending":
                    plot = plot.OrderByDescending(p => p.Cec);
                    break;
                default:
                    plot = plot.OrderByDescending(p => p.DatePlanted);
                    break;
            }

            return View(await plot.ToListAsync());
        }

        // GET: HKPlot/Details/5
        public async Task<IActionResult> Details(int? id, string where)
        {
            ViewBag.where = where;

            if (id == null)
            {
                return NotFound();
            }

            var plot = await _context.Plot
                .Include(p => p.Farm)
                .Include(p => p.Variety)
                .SingleOrDefaultAsync(m => m.PlotId == id);
            if (plot == null)
            {
                return NotFound();
            }

            return View(plot);
        }

        // GET: HKPlot/Create
        public IActionResult Create(string farmId, string varietyId, string cropId, string where)
        {
            ViewBag.where = where;

            if (!string.IsNullOrEmpty(cropId))
            {
                ViewData["VarietyId"] = new SelectList(_context.Variety.Where(v => v.CropId == Convert.ToInt16(cropId)), "VarietyId", "Name");
            }
            else
            {
                ViewData["VarietyId"] = new SelectList(_context.Variety, "VarietyId", "Name", varietyId);
            }

            ViewData["FarmId"] = new SelectList(_context.Farm, "FarmId", "Name");
            
            return View();
        }

        // POST: HKPlot/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlotId,FarmId,VarietyId,DatePlanted,DateHarvested,PlantingRate,PlantingRateByPounds,RowWidth,PatternRepeats,OrganicMatter,BicarbP,Potassium,Magnesium,Calcium,PHsoil,PHbuffer,Cec,PercentBaseSaturationK,PercentBaseSaturationMg,PercentBaseSaturationCa,PercentBaseSaturationH,Comments")] Plot plot)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FarmId"] = new SelectList(_context.Farm, "FarmId", "Name", plot.Farm.Name);
            ViewData["VarietyId"] = new SelectList(_context.Variety, "VarietyId", "Name", plot.Variety.Name);
            return View(plot);
        }

        // GET: HKPlot/Edit/5
        public async Task<IActionResult> Edit(int? id,string where)
        {
            ViewBag.where = where;

            if (id == null)
            {
                return NotFound();
            }

            var plot = await _context.Plot
                .Include(f => f.Farm)
                .Include(v => v.Variety)
                .SingleOrDefaultAsync(m => m.PlotId == id);
            if (plot == null)
            {
                return NotFound();
            }
            ViewData["FarmId"] = new SelectList(_context.Farm, "FarmId", "Name", plot.Farm.Name);
            ViewData["VarietyId"] = new SelectList(_context.Variety, "VarietyId", "Name", plot.Variety.Name);
            return View(plot);
        }

        // POST: HKPlot/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlotId,FarmId,VarietyId,DatePlanted,DateHarvested,PlantingRate,PlantingRateByPounds,RowWidth,PatternRepeats,OrganicMatter,BicarbP,Potassium,Magnesium,Calcium,PHsoil,PHbuffer,Cec,PercentBaseSaturationK,PercentBaseSaturationMg,PercentBaseSaturationCa,PercentBaseSaturationH,Comments")] Plot plot)
        {
            if (id != plot.PlotId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plot);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlotExists(plot.PlotId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["FarmId"] = new SelectList(_context.Farm.OrderBy(a=>a.Name), "FarmId", "Name", plot.Farm.Name);
            ViewData["VarietyId"] = new SelectList(_context.Variety.OrderBy(a => a.Name), "VarietyId", "Name", plot.Variety.Name);
            return View(plot);
        }

        // GET: HKPlot/Delete/5
        public async Task<IActionResult> Delete(int? id, string where)
        {
            ViewBag.where = where;

            if (id == null)
            {
                return NotFound();
            }

            var plot = await _context.Plot
                .Include(p => p.Farm)
                .Include(p => p.Variety)
                .SingleOrDefaultAsync(m => m.PlotId == id);
            if (plot == null)
            {
                return NotFound();
            }

            return View(plot);
        }

        // POST: HKPlot/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {

            var plot = await _context.Plot
                .Include(p => p.Treatment)
                .SingleOrDefaultAsync(m => m.PlotId == id);

            foreach (var treatment in plot.Treatment.ToList())
            {
                var deletObj = await _context.Treatment
                    .Include(t => t.TreatmentFertilizer)
                    .SingleOrDefaultAsync(m => m.TreatmentId == treatment.TreatmentId);
                _context.Treatment.Remove(deletObj);
            }

            _context.Plot.Remove(plot);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlotExists(int id)
        {
            return _context.Plot.Any(e => e.PlotId == id);
        }
    }
}
