using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Numerics;
using ToDoListTask.Data;
using ToDoListTask.Models;

namespace ToDoListTask.Controllers
{
    public class TaskController : Controller
    {
        readonly private AppDbContext _context;
        public TaskController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var tasks = _context.Tasks;
            return View(tasks.AsEnumerable());
        }
        public IActionResult AddTask()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddTask(ToDoTask task, IFormFile file)
        {
            if (file is not null)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }

                task.File = fileName;
            }
            _context.Tasks.Add(task);
            _context.SaveChanges();
            TempData["success"] = "Task added successfully!";
            return RedirectToAction("index");
        }
        public IActionResult Edit(int id)
        {
            var task = _context.Tasks.FirstOrDefault(x => x.Id == id);
            if (task is null) return NotFound();
            return View(task);
        }
        [HttpPost]
        public IActionResult Edit(ToDoTask task, IFormFile? file)
        {
            var taskInDb = _context.Tasks.AsNoTracking().FirstOrDefault(x => x.Id == task.Id);
            if (taskInDb is null) return NotFound();
            if (file is not null && file.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets", fileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    file.CopyTo(stream);
                }
                var OldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets", taskInDb.File);
                if (System.IO.File.Exists(OldPath))
                {
                    System.IO.File.Delete(OldPath);
                }
                task.File = fileName;
            }
            else
            {
                task.File = taskInDb.File;
            }
            _context.Tasks.Update(task);
            _context.SaveChanges();
            TempData["success"] = "Task updated successfully!";
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Delete(int id)
        {
            var task = _context.Tasks.FirstOrDefault(x => x.Id == id);
            if (task is null) return NotFound();
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\assets", task.File);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            TempData["success"] = "Task deleted successfully!";
            _context.Tasks.Remove(task);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }
    }
}
