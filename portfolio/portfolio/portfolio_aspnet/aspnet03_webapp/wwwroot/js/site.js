// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
// Trumbowyg 에디터를 초기화하는 로직
$(".editor").trumbowyg();
$(document).ready(function () {
    $('.loop').owlCarousel({
        loop: true, // 무한 루프
        margin: 30, // 항목 사이의 여백
        nav: true, // 이전/다음 버튼 표시
        dots: false, // 점 표시기 숨김
        responsive: {
            0: {
                items: 1 // 모바일 화면에서 1개의 항목만 보여줌
            },
            768: {
                items: 2 // 태블릿 화면에서 2개의 항목 보여줌
            },
            992: {
                items: 3 // 데스크톱 화면에서 3개의 항목 보여줌
            }
        }
    });
});