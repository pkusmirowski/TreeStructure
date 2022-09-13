using Microsoft.AspNetCore.Mvc;
using TreeStructure.Services;

namespace TreeStructure.Controllers
{
    public class TreeController : Controller
    {
        private readonly TreeService _treeServce;

        public TreeController(TreeService treeServce)
        {
            _treeServce = treeServce;
        }

        public IActionResult Tree()
        {
            var tree = _treeServce.DisplayTree();
            string success = (string)TempData["Success"];
            string failure = (string)TempData["Failure"];
            ViewBag.Success = success;
            ViewBag.Failure = failure;
            return View(tree);
        }

        [HttpPost]
        public async Task<IActionResult> AddElementAsync(int id, string name)
        {
            TempData["Failure"] = "Dodanie nie powiodło się.";
            if (ModelState.IsValid && await _treeServce.AddElement(id, name))
            {
                TempData["Success"] = "Dodanie powiodło się.";
                return RedirectToAction("Tree");
            }
            return RedirectToAction("Tree");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteElementAsync(int id)
        {
            TempData["Failure"] = "Usunięcie nie powiodło się.";
            if (ModelState.IsValid && await _treeServce.DeleteElement(id))
            {
                TempData["Success"] = "Usunięcie powiodło się.";
                return RedirectToAction("Tree");
            }
            return RedirectToAction("Tree");
        }

        [HttpPost]
        public async Task<IActionResult> EditElementAsync(int id, string name)
        {
            TempData["Failure"] = "Edycja nie powiodła się.";
            if (ModelState.IsValid && await _treeServce.EditElement(id, name))
            {
                TempData["Success"] = "Edycja powiodła się.";
                return RedirectToAction("Tree");
            }
            return RedirectToAction("Tree");
        }

        [HttpPost]
        public async Task<IActionResult> MoveElementAsync(int id, string newId)
        {
            TempData["Failure"] = "Przeniesienie nie powiodło się.";
            int newIdInt = Convert.ToInt32(newId);
            if (ModelState.IsValid && await _treeServce.MoveElement(id, newIdInt))
            {
                TempData["Success"] = "Przeniesienie powiodło się.";
                return RedirectToAction("Tree");
            }
            return RedirectToAction("Tree");
        }
    }
}
