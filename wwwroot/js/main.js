let modalBtns = document.querySelectorAll(".card-image .modal-btn")


modalBtns.forEach(modalBtn => {
    modalBtn.addEventListener('click', function (e) {
        e.preventDefault();

        let url = $(this).attr("href");

        fetch(url)
            .then(response => {

                if (response.ok) {
                    return response.text()
                }
                else {
                    alert("xeta bas verdi!")
                }
            })
            .then(data => {
                $("#quickModal .modal-dialog").html(data)
                $("#quickModal").modal('show')
            })
    })
})

let basketBtns = document.querySelectorAll("#addtobasket")
let basketItemCount = document.querySelector(".cart-total .text-number")
let itemContainer = document.querySelector(".cart-block")

basketBtns.forEach(basketBtn => {
    basketBtn.addEventListener('click', function (e) {
        e.preventDefault();

        let url = basketBtn.getAttribute("href")

        fetch(url)
            .then(response => {
                if (!response.ok) {
                    alert("xeta bas verdi!")
                }
                return response.text();
                //com
            })
            .then(data => {
                itemContainer.innerHTML = data
            })
    })
})


//let loadMoreButton = document.querySelector("#loadMoreButton");
//loadMoreButton.addEventListener("click", function (e) {
//    e.preventDefault();

//    let url = loadMoreButton.getAttribute("href")

//    fetch(url)
//        .then(response => response.json())
//        .then(data => {
//            data.forEach(comment => {
//                const reviewComment = document.createElement('div');
//                reviewComment.classList.add('review-comment', 'mb--20');

//                const avatar = document.createElement('div');
//                avatar.classList.add('avatar');
//                const avatarImg = document.createElement('img');
//                avatarImg.src = 'image/icon/author-logo.png';
//                avatarImg.alt = '';
//                avatar.appendChild(avatarImg);

//                const text = document.createElement('div');
//                text.classList.add('text');

//                const ratingBlock = document.createElement('div');
//                ratingBlock.classList.add('rating-block', 'mb--15');

//                for (let i = 1; i <= 5; i++) {
//                    const star = document.createElement('span');
//                    star.classList.add('ion-android-star-outline');
//                    if (i <= comment.Rate) {
//                        star.classList.add('star_on');
//                    }
//                    ratingBlock.appendChild(star);
//                }

//                const author = document.createElement('h6');
//                author.classList.add('author');
//                author.innerHTML = `${comment.AppUser.FulName} – <span class="font-weight-400">${comment.CreatedAt}</span>`;

//                const commentText = document.createElement('p');
//                commentText.innerHTML = comment.Text;

//                text.appendChild(ratingBlock);
//                text.appendChild(author);
//                text.appendChild(commentText);

//                reviewComment.appendChild(avatar);
//                reviewComment.appendChild(text);

//                const reviewWrapper = document.querySelector('.review-wrapper');
//                reviewWrapper.appendChild(reviewComment);
//            });
//        })
//        .catch(error => {
//            console.error('Error:', error);
//        });
//});