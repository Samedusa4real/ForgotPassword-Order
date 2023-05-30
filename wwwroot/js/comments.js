//let loadMoreButton = document.querySelector("#loadMoreButton")
//let comContainer = document.querySelector(".review-comment")

//let url = loadMoreButton.getAttribute("href")

//loadMoreButton.addEventListener('click', function (e) {
//    e.preventDefault();

//    fetch(url)
//        .then(response => {
//            if (!response.ok) {
//                alert("xeta bas verdi!")
//            }
//            return response.text();
//            //com
//        })
//        .then(data => {
//            comContainer.innerHTML = data
//        })
//})

//document.getElementById('loadMoreButton').addEventListener('click', function () {
//    var loadMoreCount = @(int) ViewBag.CommentsToShow + 1;
//    location.href = '@Url.Action("Detail", "Book", new { id = Model.Book.Id })?loadMoreComments=' + loadMoreCount;
//});

