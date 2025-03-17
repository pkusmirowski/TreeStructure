using Microsoft.AspNetCore.Mvc;
using TreeStructure.Models;
using TreeStructure.Services;

namespace TreeStructure.Controllers
{
    public class TreeController : Controller
    {
        private readonly ITreeService _treeService;
        private readonly ILogger<TreeController> _logger;

        public TreeController(ITreeService treeService, ILogger<TreeController> logger)
        {
            _treeService = treeService;
            _logger = logger;
        }

        public IActionResult Error()
        {
            return View();
        }

        public async Task<IActionResult> Tree()
        {
            try
            {
                var tree = await _treeService.DisplayTreeAsync();
                if (tree == null || tree.Id == 0)
                {
                    return View("Error");
                }

                var allItems = new Dictionary<int, string>();
                await PopulateAllItems(tree.InverseParent, allItems, "");
                ViewBag.Values = allItems;

                // Transfer TempData to ViewBag
                ViewBag.Success = TempData["Success"];
                ViewBag.Failure = TempData["Failure"];

                return View(tree);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas wyświetlania drzewa");
                return View("Error");
            }
        }

        private static async Task PopulateAllItems(ICollection<Tree>? children, Dictionary<int, string> list, string prefix = "")
        {
            if (children == null || !children.Any())
            {
                return;
            }

            foreach (var child in children)
            {
                list[child.Id] = $"{prefix}{child.Folder}";

                if (child.InverseParent != null && child.InverseParent.Any())
                {
                    await PopulateAllItems(child.InverseParent, list, prefix + "--");
                }
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddElementAsync(int id, string name)
        {
            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Nieprawidłowe dane.";
                return RedirectToAction("Tree");
            }

            try
            {
                if (await _treeService.AddElementAsync(id, name))
                {
                    TempData["Success"] = "Element został dodany pomyślnie.";
                    return RedirectToAction("Tree");
                }

                TempData["Failure"] = "Nie udało się dodać elementu.";
                return RedirectToAction("Tree");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas dodawania elementu");
                TempData["Failure"] = "Wystąpił błąd podczas dodawania elementu.";
                return RedirectToAction("Tree");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteElementAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Nieprawidłowe dane.";
                return RedirectToAction("Tree");
            }

            try
            {
                if (await _treeService.DeleteElementAsync(id))
                {
                    TempData["Success"] = "Element został pomyślnie usunięty.";
                    return RedirectToAction("Tree");
                }

                TempData["Failure"] = "Nie udało się usunąć elementu.";
                return RedirectToAction("Tree");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas usuwania elementu");
                TempData["Failure"] = "Wystąpił błąd podczas usuwania elementu.";
                return RedirectToAction("Tree");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditElementAsync(int id, string name)
        {
            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Nieprawidłowe dane.";
                return RedirectToAction("Tree");
            }

            try
            {
                if (await _treeService.EditElementAsync(id, name))
                {
                    TempData["Success"] = "Element został edytowany pomyślnie.";
                    return RedirectToAction("Tree");
                }

                TempData["Failure"] = "Nie udało się edytować elementu.";
                return RedirectToAction("Tree");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas edytowania elementu");
                TempData["Failure"] = "Wystąpił błąd podczas edytowania elementu.";
                return RedirectToAction("Tree");
            }
        }

        [HttpPost]
        public async Task<IActionResult> MoveElementAsync(int id, string newId)
        {
            if (!int.TryParse(newId, out var newIdInt))
            {
                TempData["Failure"] = "Nieprawidłowy nowy identyfikator.";
                return RedirectToAction("Tree");
            }

            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Nieprawidłowe dane.";
                return RedirectToAction("Tree");
            }

            try
            {
                if (await _treeService.MoveElementAsync(id, newIdInt))
                {
                    TempData["Success"] = "Element został przeniesiony pomyślnie.";
                    return RedirectToAction("Tree");
                }

                TempData["Failure"] = "Nie udało się przenieść elementu.";
                return RedirectToAction("Tree");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Błąd podczas przenoszenia elementu");
                TempData["Failure"] = "Wystąpił błąd podczas przenoszenia elementu.";
                return RedirectToAction("Tree");
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddNode(int parentId, string nodeName)
        {
            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Nieprawidłowe dane.";
                return RedirectToAction("Tree");
            }

            if (await _treeService.AddNodeAsync(parentId, nodeName))
            {
                TempData["Success"] = "Węzeł został dodany pomyślnie.";
            }
            else
            {
                TempData["Failure"] = "Nie udało się dodać węzła.";
            }

            return RedirectToAction("Tree");
        }

        [HttpGet]
        public async Task<IActionResult> Search(string query)
        {
            var results = await _treeService.SearchTreeAsync(query);
            ViewBag.SearchResults = results;
            var tree = await _treeService.DisplayTreeAsync();
            return View("Tree", tree);
        }
    }
}
