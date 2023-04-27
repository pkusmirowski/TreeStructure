using Microsoft.AspNetCore.Mvc;
using TreeStructure.Services;

namespace TreeStructure.Controllers
{
    public class TreeController : Controller
    {
        private readonly TreeService _treeService;

        public TreeController(TreeService treeService)
        {
            _treeService = treeService;
        }

        public IActionResult Error()
        {
            return View();
        }

        public IActionResult Tree()
        {
            var tree = _treeService.DisplayTree();
            if (tree == null || tree.Id == 0)
            {
                return View("Error");
            }

            ViewBag.Success = TempData["Success"];
            ViewBag.Failure = TempData["Failure"];

            return View(tree);
        }

        [HttpPost]
        public async Task<IActionResult> AddElementAsync(int id, string name)
        {
            if (ModelState.IsValid && await _treeService.AddElement(id, name))
            {
                TempData["Success"] = "Dodanie powiodło się.";
                return RedirectToAction("Tree");
            }

            TempData["Failure"] = "Dodanie nie powiodło się.";
            return RedirectToAction("Tree");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteElementAsync(int id)
        {
            if (ModelState.IsValid && await _treeService.DeleteElement(id))
            {
                TempData["Success"] = "Usunięcie powiodło się.";
                return RedirectToAction("Tree");
            }

            TempData["Failure"] = "Usunięcie nie powiodło się.";
            return RedirectToAction("Tree");
        }

        [HttpPost]
        public async Task<IActionResult> EditElementAsync(int id, string name)
        {
            if (ModelState.IsValid && await _treeService.EditElement(id, name))
            {
                TempData["Success"] = "Edycja powiodła się.";
                return RedirectToAction("Tree");
            }

            TempData["Failure"] = "Edycja nie powiodła się.";
            return RedirectToAction("Tree");
        }

        [HttpPost]
        public async Task<IActionResult> MoveElementAsync(int id, string newId)
        {
            int newIdInt = Convert.ToInt32(newId);
            if (ModelState.IsValid && await _treeService.MoveElement(id, newIdInt))
            {
                TempData["Success"] = "Przeniesienie powiodło się.";
                return RedirectToAction("Tree");
            }

            TempData["Failure"] = "Przeniesienie nie powiodło się.";
            return RedirectToAction("Tree");
        }
    }
}
