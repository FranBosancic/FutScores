document.addEventListener("DOMContentLoaded", () => {
	const debounce = (callback, delay) => {
		let timeoutId;

		return (...args) => {
			window.clearTimeout(timeoutId);
			timeoutId = window.setTimeout(() => callback(...args), delay);
		};
	};

	const getPreferredLocale = () => {
		const languages = Array.isArray(window.navigator.languages) && window.navigator.languages.length > 0
			? window.navigator.languages
			: [window.navigator.language || "en-US"];

		return languages.find((language) => typeof language === "string" && language.length > 0) || "en-US";
	};

	const preferredLocale = getPreferredLocale();
	const isCroatianLocale = preferredLocale.toLowerCase().startsWith("hr");
	const transitionDelay = 180;

	const animateAutocompleteOpen = (resultsElement) => {
		resultsElement.classList.add("autocomplete-panel");
		resultsElement.classList.remove("hidden");
		window.requestAnimationFrame(() => {
			resultsElement.classList.add("is-open");
		});
	};

	const animateAutocompleteClose = (resultsElement) => {
		if (resultsElement.classList.contains("hidden")) {
			return;
		}

		resultsElement.classList.remove("is-open");
		window.setTimeout(() => {
			if (!resultsElement.classList.contains("is-open")) {
				resultsElement.classList.add("hidden");
			}
		}, transitionDelay);
	};

	const animateFilterRefresh = (resultsElement, html) => {
		resultsElement.classList.add("live-filter-panel", "is-loading");

		window.setTimeout(() => {
			resultsElement.innerHTML = html;
			resultsElement.classList.remove("is-loading");
			resultsElement.classList.add("is-entering");

			window.setTimeout(() => {
				resultsElement.classList.remove("is-entering");
			}, 220);
		}, 90);
	};

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

	document.querySelectorAll("[data-autocomplete-root]").forEach((root) => {
		const input = root.querySelector("[data-autocomplete-input]");
		const hiddenInput = root.querySelector("[data-autocomplete-hidden]");
		const results = root.querySelector("[data-autocomplete-results]");
		const searchUrl = root.getAttribute("data-search-url");
		const emptyStateText = root.getAttribute("data-empty-state") || "No matches found.";
		let selectedLabel = input instanceof HTMLInputElement ? input.value.trim() : "";

		if (!(input instanceof HTMLInputElement) || !(hiddenInput instanceof HTMLInputElement) || !(results instanceof HTMLDivElement) || !searchUrl) {
			return;
		}

		results.classList.add("autocomplete-panel");

		const hideResults = () => {
			results.innerHTML = "";
			animateAutocompleteClose(results);
		};

		const showEmptyState = () => {
			results.innerHTML = `<div class="px-4 py-3 text-sm text-slate-400">${emptyStateText}</div>`;
			animateAutocompleteOpen(results);
		};

		const selectOption = (option) => {
			hiddenInput.value = String(option.id);
			input.value = option.label;
			selectedLabel = option.label;
			hideResults();
			if (window.jQuery) {
				window.jQuery(hiddenInput).valid();
			}
		};

		const renderResults = (items) => {
			if (!Array.isArray(items) || items.length === 0) {
				showEmptyState();
				return;
			}

			results.innerHTML = "";

			items.forEach((item) => {
				const button = document.createElement("button");
				button.type = "button";
				button.className = "block w-full border-b border-white/10 px-4 py-3 text-left text-sm text-slate-100 transition last:border-b-0 hover:bg-white/10";
				button.textContent = item.label;
				button.addEventListener("click", () => selectOption(item));
				results.appendChild(button);
			});

			animateAutocompleteOpen(results);
		};

		const loadResults = debounce(async () => {
			const query = input.value.trim();

			if (query.length < 2) {
				hideResults();
				return;
			}

			try {
				const response = await fetch(`${searchUrl}?q=${encodeURIComponent(query)}`, {
					headers: {
						"X-Requested-With": "XMLHttpRequest"
					}
				});

				if (!response.ok) {
					hideResults();
					return;
				}

				const items = await response.json();
				renderResults(items);
			} catch {
				hideResults();
			}
		}, 250);

		input.addEventListener("input", () => {
			if (input.value.trim() !== selectedLabel) {
				hiddenInput.value = "";
			}

			loadResults();
		});

		input.addEventListener("blur", () => {
			window.setTimeout(() => {
				if (hiddenInput.value) {
					input.value = selectedLabel;
				}
				else if (window.jQuery) {
					window.jQuery(hiddenInput).valid();
				}

				hideResults();
			}, 150);
		});

		input.addEventListener("focus", () => {
			if (input.value.trim().length >= 2) {
				loadResults();
			}
		});

		document.addEventListener("click", (event) => {
			if (!root.contains(event.target)) {
				hideResults();
			}
		});
	});

	document.querySelectorAll("[data-live-filter-input]").forEach((inputElement) => {
		if (!(inputElement instanceof HTMLInputElement)) {
			return;
		}

		const filterUrl = inputElement.getAttribute("data-filter-url");
		const targetId = inputElement.getAttribute("data-filter-target");
		const resultsElement = targetId ? document.getElementById(targetId) : null;

		if (!filterUrl || !(resultsElement instanceof HTMLDivElement)) {
			return;
		}

		resultsElement.classList.add("live-filter-panel");

		const loadFilteredResults = debounce(async () => {
			const query = inputElement.value.trim();
			const targetUrl = query
				? `${filterUrl}?q=${encodeURIComponent(query)}`
				: filterUrl;

			try {
				const response = await fetch(targetUrl, {
					headers: {
						"X-Requested-With": "XMLHttpRequest"
					}
				});

				if (!response.ok) {
					return;
				}

				const html = await response.text();
				animateFilterRefresh(resultsElement, html);
				const url = query
					? `${window.location.pathname}?q=${encodeURIComponent(query)}`
					: window.location.pathname;
				window.history.replaceState({}, "", url);
			} catch {
				// Keep the current results in place if the AJAX request fails.
			}
		}, 250);

		inputElement.addEventListener("input", () => {
			loadFilteredResults();
		});
	});

	if (window.jQuery?.validator) {
		window.jQuery.validator.setDefaults({
			ignore: []
		});
	}

	if (window.flatpickr) {
		document.querySelectorAll("[data-date-time-picker]").forEach((inputElement) => {
			if (!(inputElement instanceof HTMLInputElement)) {
				return;
			}

			const enableTime = inputElement.getAttribute("data-enable-time") !== "false";
			const minuteIncrement = Number.parseInt(inputElement.getAttribute("data-minute-increment") || "5", 10);

			window.flatpickr(inputElement, {
				allowInput: true,
				altInput: true,
				altInputClass: "w-full rounded-2xl border border-white/10 bg-white/5 px-4 py-3 text-slate-100 outline-none transition placeholder:text-slate-500 focus:border-lime-400/60 focus:bg-white/10",
				altFormat: enableTime
					? (isCroatianLocale ? "d.m.Y. H:i" : "m/d/Y h:i K")
					: (isCroatianLocale ? "d.m.Y." : "m/d/Y"),
				dateFormat: enableTime ? "Y-m-d\\TH:i:S" : "Y-m-d",
				enableTime,
				time_24hr: isCroatianLocale,
				locale: isCroatianLocale && window.flatpickr.l10ns.hr ? window.flatpickr.l10ns.hr : "default",
				minuteIncrement: Number.isNaN(minuteIncrement) ? 5 : minuteIncrement,
				onReady: (_, __, instance) => {
					instance.altInput?.setAttribute("placeholder", inputElement.getAttribute("placeholder") || "Select date and time");
				},
				onClose: (_, __, instance) => {
					if (window.jQuery) {
						window.jQuery(instance.input).valid();
					}
				}
			});
		});
	}

	const getRatingToneClass = (score) => {
		if (score >= 9) {
			return "border-emerald-400/20 bg-emerald-500/10 text-emerald-200";
		}

		if (score >= 7) {
			return "border-lime-400/20 bg-lime-500/10 text-lime-200";
		}

		if (score >= 5) {
			return "border-amber-400/20 bg-amber-500/10 text-amber-200";
		}

		return "border-rose-400/20 bg-rose-500/10 text-rose-200";
	};

	const getRatingLabel = (score) => {
		if (score >= 9) {
			return "Standout display";
		}

		if (score >= 7) {
			return "Strong performance";
		}

		if (score >= 5) {
			return "Mixed return";
		}

		return "Underwhelming night";
	};

	document.querySelectorAll("[data-rating-score-root]").forEach((root) => {
		const rangeInput = root.querySelector("[data-rating-score-range]");
		const numberInput = root.querySelector("[data-rating-score-input]");
		const scoreChip = document.querySelector("[data-rating-score-chip]");
		const scoreLabel = document.querySelector("[data-rating-score-label]");

		if (!(rangeInput instanceof HTMLInputElement) || !(numberInput instanceof HTMLInputElement) || !(scoreChip instanceof HTMLElement) || !(scoreLabel instanceof HTMLElement)) {
			return;
		}

		const toneClasses = [
			"border-emerald-400/20", "bg-emerald-500/10", "text-emerald-200",
			"border-lime-400/20", "bg-lime-500/10", "text-lime-200",
			"border-amber-400/20", "bg-amber-500/10", "text-amber-200",
			"border-rose-400/20", "bg-rose-500/10", "text-rose-200"
		];

		const syncScore = (rawValue) => {
			const parsed = Number.parseInt(rawValue, 10);
			const score = Number.isNaN(parsed) ? 1 : Math.min(10, Math.max(1, parsed));

			rangeInput.value = String(score);
			numberInput.value = String(score);
			scoreChip.textContent = String(score);
			scoreLabel.textContent = getRatingLabel(score);
			scoreChip.classList.remove(...toneClasses);
			scoreChip.classList.add(...getRatingToneClass(score).split(" "));
		};

		syncScore(numberInput.value || rangeInput.value);

		rangeInput.addEventListener("input", () => {
			syncScore(rangeInput.value);
			if (window.jQuery) {
				window.jQuery(numberInput).valid();
			}
		});

		numberInput.addEventListener("input", () => {
			syncScore(numberInput.value);
		});

		numberInput.addEventListener("blur", () => {
			syncScore(numberInput.value);
			if (window.jQuery) {
				window.jQuery(numberInput).valid();
			}
		});
	});

	document.querySelectorAll("[data-rating-comment-input]").forEach((inputElement) => {
		if (!(inputElement instanceof HTMLTextAreaElement)) {
			return;
		}

		const counter = inputElement.closest("div")?.querySelector("[data-rating-comment-count]");

		if (!(counter instanceof HTMLElement)) {
			return;
		}

		const syncCount = () => {
			counter.textContent = String(inputElement.value.length);
		};

		syncCount();
		inputElement.addEventListener("input", syncCount);
		inputElement.addEventListener("blur", () => {
			if (window.jQuery) {
				window.jQuery(inputElement).valid();
			}
		});
	});
});
