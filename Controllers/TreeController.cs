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

            return View(tree);
        }

        [HttpPost]
        public async Task<IActionResult> AddElementAsync(int id, string name)
        {
            if (ModelState.IsValid && await _treeServce.AddElement(id, name))
            {
                return RedirectToAction("Tree");
            }
            return RedirectToAction("Tree");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteElementAsync(int id)
        {
            if (ModelState.IsValid && await _treeServce.DeleteElement(id))
            {
                return RedirectToAction("Tree");
            }
            return RedirectToAction("Tree");
        }

        [HttpPost]
        public async Task<IActionResult> EditElementAsync(int id, string name)
        {
            if (ModelState.IsValid && await _treeServce.EditElement(id, name))
            {
                return RedirectToAction("Tree");
            }
            return RedirectToAction("Tree");
        }

        [HttpPost]
        public IActionResult MoveElement(int id, string newId)
        {
            int newIdInt = Convert.ToInt32(newId);
            if (ModelState.IsValid && _treeServce.MoveElement(id, newIdInt))
            {
                return RedirectToAction("Tree");
            }
            return RedirectToAction("Tree");
        }
    }
}
