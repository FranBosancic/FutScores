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
			// The menu is hidden via the Tailwind `hidden` class, so toggle that
			// (not the `hidden` attribute) to show/hide it.
			const isNowOpen = mobileMenu.classList.toggle("hidden") === false;
			mobileToggle.setAttribute("aria-expanded", isNowOpen ? "true" : "false");
		});
	}

	// ── Clickable rows ──
	// A row carrying data-row-href navigates there when clicked, except when the
	// click lands on a real control (link, button, form field) so those keep their
	// own behaviour. Delegated on document so AJAX-refreshed rows work too.
	document.addEventListener("click", (event) => {
		const target = event.target;

		if (!(target instanceof Element) || target.closest("a, button, input, select, textarea, label")) {
			return;
		}

		const row = target.closest("[data-row-href]");
		const href = row?.getAttribute("data-row-href");

		if (href) {
			window.location.assign(href);
		}
	});

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
			// filterUrl may already carry a query string (e.g. ?leagueId=2), so choose
			// the correct separator instead of blindly appending "?", which would
			// produce "?leagueId=2?q=…" and silently drop both the scope and the query.
			const separator = filterUrl.includes("?") ? "&" : "?";
			const targetUrl = query
				? `${filterUrl}${separator}q=${encodeURIComponent(query)}`
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

				// Update only the q param in the address bar, preserving the rest
				// (e.g. leagueId) so a reload keeps the same league scope.
				const params = new URLSearchParams(window.location.search);
				if (query) {
					params.set("q", query);
				} else {
					params.delete("q");
				}
				const queryString = params.toString();
				const url = queryString
					? `${window.location.pathname}?${queryString}`
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

	// ── Dependent <select> cascade (e.g. League → Home → Away → Match → Player) ──
	// Each dependent select declares the keys it depends on, the endpoint to fetch
	// its options from, and how to map parent values onto query params. Changing a
	// step rebuilds every step below it, so a stale downstream choice can't survive.
	document.querySelectorAll("[data-cascade-chain]").forEach((chain) => {
		const selects = {};
		// Document order == chain order (league, home, away, match, player).
		const keys = [];
		chain.querySelectorAll("select[data-cascade-key]").forEach((select) => {
			const key = select.getAttribute("data-cascade-key");
			selects[key] = select;
			keys.push(key);
		});

		if (keys.length === 0) {
			return;
		}

		const configOf = (select) => {
			const dependsRaw = select.getAttribute("data-cascade-depends") || "";
			const paramsRaw = select.getAttribute("data-cascade-params") || "";

			return {
				key: select.getAttribute("data-cascade-key"),
				url: select.getAttribute("data-cascade-url"),
				placeholder: select.getAttribute("data-cascade-placeholder") || "Select an option",
				depends: dependsRaw.split(",").map((value) => value.trim()).filter(Boolean),
				params: paramsRaw
					.split(",")
					.map((pair) => pair.split(":"))
					.filter((parts) => parts.length === 2)
					.map(([param, sourceKey]) => ({ param: param.trim(), sourceKey: sourceKey.trim() }))
			};
		};

		// Steps that fetch their own options, kept in chain order.
		const dependents = keys
			.map((key) => selects[key])
			.filter((select) => select.getAttribute("data-cascade-url"))
			.map(configOf);

		const valueOf = (key) => (selects[key] ? selects[key].value : "");

		const addPlaceholder = (select, text) => {
			const placeholder = document.createElement("option");
			placeholder.value = "";
			placeholder.textContent = text;
			select.appendChild(placeholder);
		};

		const resetSelect = (config) => {
			const select = selects[config.key];
			select.innerHTML = "";
			addPlaceholder(select, config.placeholder);
			select.value = "";
			select.disabled = true;
		};

		const populateSelect = (config, options) => {
			const select = selects[config.key];
			select.innerHTML = "";
			addPlaceholder(select, config.placeholder);

			let currentGroupName = null;
			let currentGroupEl = null;

			(Array.isArray(options) ? options : []).forEach((option) => {
				const optionEl = document.createElement("option");
				optionEl.value = String(option.id);
				optionEl.textContent = option.label;

				if (option.group) {
					if (option.group !== currentGroupName) {
						currentGroupName = option.group;
						currentGroupEl = document.createElement("optgroup");
						currentGroupEl.label = option.group;
						select.appendChild(currentGroupEl);
					}

					currentGroupEl.appendChild(optionEl);
				} else {
					currentGroupName = null;
					currentGroupEl = null;
					select.appendChild(optionEl);
				}
			});

			select.value = "";
			select.disabled = false;
		};

		const loadDependent = async (config) => {
			const ready = config.depends.every((key) => valueOf(key));

			if (!ready || !config.url) {
				resetSelect(config);
				return;
			}

			const query = config.params
				.map(({ param, sourceKey }) => `${encodeURIComponent(param)}=${encodeURIComponent(valueOf(sourceKey))}`)
				.join("&");

			try {
				const response = await fetch(query ? `${config.url}?${query}` : config.url, {
					headers: { "X-Requested-With": "XMLHttpRequest" }
				});

				if (!response.ok) {
					resetSelect(config);
					return;
				}

				populateSelect(config, await response.json());
			} catch {
				resetSelect(config);
			}
		};

		const refreshBelow = async (changedKey) => {
			const changedIndex = keys.indexOf(changedKey);
			const below = dependents.filter((config) => keys.indexOf(config.key) > changedIndex);

			// Sequential so each step sees its parent already reset/loaded.
			for (const config of below) {
				await loadDependent(config);
			}
		};

		keys.forEach((key) => {
			selects[key].addEventListener("change", () => {
				if (window.jQuery) {
					window.jQuery(selects[key]).valid();
				}

				refreshBelow(key);
			});
		});
	});
});
