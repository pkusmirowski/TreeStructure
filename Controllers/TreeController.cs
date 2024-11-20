using Microsoft.AspNetCore.Mvc;
using TreeStructure.Models;
using TreeStructure.Services;

namespace TreeStructure.Controllers
{
    public class TreeController : Controller
    {
        private readonly ITreeService _treeService;

        public TreeController(ITreeService treeService)
        {
            _treeService = treeService;
        }

        public IActionResult Error()
        {
            return View();
        }

        public async Task<IActionResult> Tree()
        {
            var tree = await _treeService.DisplayTreeAsync();
            if (tree == null || tree.Id == 0)
            {
                return View("Error");
            }

            var allItems = new Dictionary<int, string>();
            await PopulateAllItems(tree.InverseParent, allItems, "");
            ViewBag.Values = allItems;

            ViewBag.Success = TempData["Success"];
            ViewBag.Failure = TempData["Failure"];

            return View(tree);
        }

        private static async Task PopulateAllItems(ICollection<Tree>? children, Dictionary<int, string> lista, string prefix = "")
        {
            if (children == null || !children.Any())
            {
                return;
            }

            foreach (var child in children)
            {
                // Dodanie prefiksu do nazwy elementu (symbolizuje poziom hierarchii)
                lista[child.Id] = $"{prefix}{child.Folder}";

                if (child.InverseParent != null && child.InverseParent.Any())
                {
                    await PopulateAllItems(child.InverseParent, lista, prefix + "--");
                }
            }
        }


        [HttpPost]
        public async Task<IActionResult> AddElementAsync(int id, string name)
        {
            if (ModelState.IsValid && await _treeService.AddElementAsync(id, name))
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
            if (ModelState.IsValid && await _treeService.DeleteElementAsync(id))
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
            if (ModelState.IsValid && await _treeService.EditElementAsync(id, name))
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
            if (!int.TryParse(newId, out var newIdInt))
            {
                TempData["Failure"] = "Przeniesienie nie powiodło się – nieprawidłowy identyfikator.";
                return RedirectToAction("Tree");
            }

            if (ModelState.IsValid && await _treeService.MoveElementAsync(id, newIdInt))
            {
                TempData["Success"] = "Przeniesienie powiodło się.";
                return RedirectToAction("Tree");
            }

            TempData["Failure"] = "Przeniesienie nie powiodło się.";
            return RedirectToAction("Tree");
        }
    }
}
