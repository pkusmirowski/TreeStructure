using Microsoft.AspNetCore.Mvc;
using TreeStructure.Models;
using TreeStructure.Services;
using Microsoft.Extensions.Logging;

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

                ViewBag.Success = TempData["Success"];
                ViewBag.Failure = TempData["Failure"];

                return View(tree);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error displaying tree");
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
                TempData["Failure"] = "Invalid data.";
                return RedirectToAction("Tree");
            }

            try
            {
                if (await _treeService.AddElementAsync(id, name))
                {
                    TempData["Success"] = "Element added successfully.";
                    return RedirectToAction("Tree");
                }

                TempData["Failure"] = "Failed to add element.";
                return RedirectToAction("Tree");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding element");
                TempData["Failure"] = "An error occurred while adding the element.";
                return RedirectToAction("Tree");
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteElementAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Invalid data.";
                return RedirectToAction("Tree");
            }

            try
            {
                if (await _treeService.DeleteElementAsync(id))
                {
                    TempData["Success"] = "Element deleted successfully.";
                    return RedirectToAction("Tree");
                }

                TempData["Failure"] = "Failed to delete element.";
                return RedirectToAction("Tree");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting element");
                TempData["Failure"] = "An error occurred while deleting the element.";
                return RedirectToAction("Tree");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditElementAsync(int id, string name)
        {
            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Invalid data.";
                return RedirectToAction("Tree");
            }

            try
            {
                if (await _treeService.EditElementAsync(id, name))
                {
                    TempData["Success"] = "Element edited successfully.";
                    return RedirectToAction("Tree");
                }

                TempData["Failure"] = "Failed to edit element.";
                return RedirectToAction("Tree");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error editing element");
                TempData["Failure"] = "An error occurred while editing the element.";
                return RedirectToAction("Tree");
            }
        }

        [HttpPost]
        public async Task<IActionResult> MoveElementAsync(int id, string newId)
        {
            if (!int.TryParse(newId, out var newIdInt))
            {
                TempData["Failure"] = "Invalid new ID.";
                return RedirectToAction("Tree");
            }

            if (!ModelState.IsValid)
            {
                TempData["Failure"] = "Invalid data.";
                return RedirectToAction("Tree");
            }

            try
            {
                if (await _treeService.MoveElementAsync(id, newIdInt))
                {
                    TempData["Success"] = "Element moved successfully.";
                    return RedirectToAction("Tree");
                }

                TempData["Failure"] = "Failed to move element.";
                return RedirectToAction("Tree");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error moving element");
                TempData["Failure"] = "An error occurred while moving the element.";
                return RedirectToAction("Tree");
            }
        }
    }
}
