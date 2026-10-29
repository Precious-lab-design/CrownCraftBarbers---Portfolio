// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
document.addEventListener("DOMContentLoaded", function () {

    const menuButton =
        document.getElementById("mobileMenuButton");

    const navLinks =
        document.getElementById("navLinks");


    if (menuButton && navLinks) {

        menuButton.addEventListener("click", function () {

            navLinks.classList.toggle("active");

        });


        const links =
            navLinks.querySelectorAll("a");


        links.forEach(function (link) {

            link.addEventListener("click", function () {

                navLinks.classList.remove("active");

            });

        });

    }

});

// =========================================
// WELCOME MODAL
// =========================================

document.addEventListener("DOMContentLoaded", function () {

    const welcomeModal = document.getElementById("welcomeModal");
    const welcomeModalClose = document.getElementById("welcomeModalClose");
    const welcomeModalDismiss = document.getElementById("welcomeModalDismiss");
    const welcomeModalOverlay = document.querySelector(".welcome-modal-overlay");

    if (!welcomeModal) {
        return;
    }

    function openWelcomeModal() {
        welcomeModal.classList.add("is-visible");
        welcomeModal.setAttribute("aria-hidden", "false");
        document.body.classList.add("modal-open");
    }

    function closeWelcomeModal() {
        welcomeModal.classList.remove("is-visible");
        welcomeModal.setAttribute("aria-hidden", "true");
        document.body.classList.remove("modal-open");

        sessionStorage.setItem("welcomeModalShown", "true");
    }

    // Only show once per browser session
    const modalAlreadyShown =
        sessionStorage.getItem("welcomeModalShown");

    if (!modalAlreadyShown) {
        setTimeout(function () {
            openWelcomeModal();
        }, 1200);
    }

    // X button
    if (welcomeModalClose) {
        welcomeModalClose.addEventListener("click", closeWelcomeModal);
    }

    // Maybe later button
    if (welcomeModalDismiss) {
        welcomeModalDismiss.addEventListener("click", closeWelcomeModal);
    }

    // Clicking outside the modal closes it
    if (welcomeModalOverlay) {
        welcomeModalOverlay.addEventListener("click", closeWelcomeModal);
    }

    // ESC key closes the modal
    document.addEventListener("keydown", function (event) {
        if (event.key === "Escape" &&
            welcomeModal.classList.contains("is-visible")) {

            closeWelcomeModal();
        }
    });

});