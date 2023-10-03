using Lab3.Models;
using Microsoft.AspNetCore.Mvc;

namespace Lab3.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult SongForm()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return Error();
            }
        }

        public IActionResult Index()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return Error();
            }
        }

        [HttpPost]
        public IActionResult Sing(int numberOfMonkeys)
        {
            try
            {
                if (numberOfMonkeys < 50 || numberOfMonkeys > 100)
                {
                    ModelState.AddModelError("numberOfMonkeys", "The number of monkeys must be between 50 and 100 (inclusive).");
                    return View("SongForm");
                }

                ViewData["NumberOfMonkeys"] = numberOfMonkeys;
                return View("Sing", numberOfMonkeys);
            }
            catch (Exception ex)
            {
                return Error();
            }
        }

        public IActionResult CreateStudent()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                return Error();
            }
        }

        [HttpPost]
        public IActionResult DisplayStudent(Student student)
        {
            try
            {
                return View("DisplayStudent", student);
            }
            catch (Exception ex)
            {
                return Error();
            }
        }

        public IActionResult Error()
        {
            return View("Error");
        }
    }
}