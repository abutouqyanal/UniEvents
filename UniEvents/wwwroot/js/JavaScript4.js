const USERID = {
    name: null,
    identity: null,
    image: null,
    message: null,
    date: null
}

const userComment = document.querySelector(".usercomment");
const publishBtn = document.querySelector("#publish");
const comments = document.querySelector(".comments");
const userName = document.querySelector(".user");
const notify = document.querySelector(".notifyinput");

    userComment.addEventListener("input", e => {
        if(!userComment.value) {
            publishBtn.setAttribute("disabled", "disabled");
            publishBtn.classList.remove("abled")
        }else {
            publishBtn.removeAttribute("disabled");
            publishBtn.classList.add("abled")
        }
    })

    function addPost() {
        if(!userComment.value) return;
        USERID.name = userName.value;
        if(USERID.name === "Anonymous") {
            USERID.identity = false;
            USERID.image = "photo/Screenshot 2025-02-21 180048.png"
        }else {
            USERID.identity = true;
            USERID.image = "user.png"
        }

        USERID.message = userComment.value;
        USERID.date = new Date().toLocaleString();
        let published = 
        `<div class="parents">
            <img src="${USERID.image}">
            <div>
                <h1>${USERID.name}</h1>
                <p>${USERID.message}</p>
                <div class="engagements"><img src="like.png" id="like"><img src="share.png" alt=""></div>
                <span class="date">${USERID.date}</span>
            </div>    
        </div>`

        comments.innerHTML += published;
        userComment.value = "";
        publishBtn.classList.remove("abled")

        let commentsNum = document.querySelectorAll(".parents").length;
        document.getElementById("comment").textContent = commentsNum;

    }

publishBtn.addEventListener("click", addPost);

<script>
    let currentSlide = 0;
    const slides = document.querySelectorAll('.carousel-image');
    const indicators = document.querySelectorAll('.indicator');

    function showSlide(index) {
        slides.forEach((slide, i) => {
            slide.style.display = i === index ? 'block' : 'none';
        });

        indicators.forEach((ind, i) => {
        ind.classList.toggle('active', i === index);
        });

    currentSlide = index;
    }

    function goToSlide(index) {
        showSlide(index);
    }

    // عرض أول صورة عند التحميل
    document.addEventListener('DOMContentLoaded', () => {
        showSlide(0);
    });
</script>
