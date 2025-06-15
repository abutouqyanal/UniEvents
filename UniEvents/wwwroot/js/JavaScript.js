const slides = document.querySelector('.carousel-slides');

const indicators = document.querySelectorAll('.indicator');
let currentIndex = 0;
const totalSlides = indicators.length;

function updateCarousel() {
    slides.style.transform = `translateX(-${currentIndex * 100}%)`;
    indicators.forEach((indicator, index) => {
        if (index === currentIndex) {
            indicator.classList.add('active');
        
        } else {
            indicator.classList.remove('active');
        }
    });
}

function goToSlide(index) {
    currentIndex = index;
    updateCarousel();
}

function autoSlide() {
    currentIndex = (currentIndex + 1) % totalSlides;
    updateCarousel();
}

setInterval(autoSlide, 3000);





