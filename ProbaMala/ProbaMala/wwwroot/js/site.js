document.addEventListener("DOMContentLoaded", () => {
	const mobileToggle = document.getElementById("mobile-menu-btn");
	const mobileMenu = document.getElementById("mobile-menu");

	if (mobileToggle && mobileMenu) {
		mobileToggle.addEventListener("click", () => {
			const isHidden = mobileMenu.hasAttribute("hidden");

			if (isHidden) {
				mobileMenu.removeAttribute("hidden");
				mobileToggle.setAttribute("aria-expanded", "true");
			} else {
				mobileMenu.setAttribute("hidden", "hidden");
				mobileToggle.setAttribute("aria-expanded", "false");
			}
		});
	}

	document.querySelectorAll("[data-search-form]").forEach((form) => {
		form.addEventListener("submit", (event) => {
			event.preventDefault();

			const scopeInput = form.querySelector("[data-search-scope]");
			const queryInput = form.querySelector("[data-search-query]");

			if (!(scopeInput instanceof HTMLSelectElement) || !(queryInput instanceof HTMLInputElement)) {
				return;
			}

			const scope = scopeInput.value || "Match";
			const query = queryInput.value.trim();
			const targetUrl = query
				? `/${scope}/Index?q=${encodeURIComponent(query)}`
				: `/${scope}/Index`;

			window.location.assign(targetUrl);
		});
	});
});
