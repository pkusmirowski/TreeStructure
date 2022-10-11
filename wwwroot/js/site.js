var toggler = document.getElementsByClassName("caret");
for (i = 0; i < toggler.length; i++) {
  toggler[i].addEventListener("click", function () {
    this.parentElement.querySelector(".nested").classList.toggle("active");
    this.classList.toggle("caret-down");
  });
}

$(document).ready(function () {
  $("#yourContainer").on("click", "#yourSpanID", function () {});
});

document.querySelector("#displayALL").onclick = function () {
  const test = document.getElementsByClassName("nested");
  if (getComputedStyle(test[0], null).display === "none") {
    for (i = 0; i < test.length; i++) {
      test[i].style.display = "block";
    }
  } else {
    for (i = 0; i < test.length; i++) {
      test[i].style.display = "none";
    }
  }
};

const btn = document.getElementById("displayALL");

btn.addEventListener("click", function handleClick() {
  const initialText = "Rozwiń całość";

  if (btn.textContent.toLowerCase().includes(initialText.toLowerCase())) {
    btn.textContent = "Zwiń całość";
  } else {
    btn.textContent = initialText;
  }
});
