# TODO

## 📋 UI Codebase TODO List (Senior Engineer Review)

### 🏗️ **ARCHITECTURE** (High Impact)

1. **Extract image discovery logic into a service layer**
   - *What:* Create `ImageService` class to handle `get_images()`, caching, and file watching
   - *Why:* `AppBody` mixes UI rendering with I/O; prevents testing, reusability, and thread-safe operations
   - *Impact:* **HIGH** — Blocks scaling and testability

2. **Implement proper state management with pub/sub pattern**
   - *What:* Create `AppStateManager` that `AppHeader` and `AppBody` both subscribe to; use callbacks for updates
   - *Why:* Currently `AppState` is just a dataclass; `AppHeader` mutates state, but `AppBody` never reads it. No synchronization between components.
   - *Impact:* **HIGH** — Data flow is broken; components can't react to state changes

3. **Break coupling between `AppHeader` and `AppBody`**
   - *What:* `AppHeader` runs engine → outputs to `IMAGE_TEMP_FOLDER` → `AppBody` watches it. Make this explicit via event system.
   - *Why:* Silent dependency; hard to understand data flow or test independently
   - *Impact:* **HIGH** — Critical for maintainability

4. **Create abstract interface for file operations**
   - *What:* Define `IImageRepository` with `get_images()`, `watch_folder()` methods
   - *Why:* Hard-coded `Path.rglob()`, config values; can't test, swap implementations, or handle errors uniformly
   - *Impact:* **MEDIUM** — Enables dependency injection and testing

### ⚡ **PERFORMANCE** (High Impact)

1. **Move image loading off the main UI thread**
   - *What:* Use `threading` or `asyncio` in `AppBody.update_ui()` to load/decode images in background
   - *Why:* `Image.open()` and `CTkImage` creation block UI during large batches; app freezes visibly
   - *Impact:* **HIGH** — Core responsiveness issue

2. **Implement image caching to prevent reloading identical paths**
   - *What:* Cache `CTkImage` objects by `Path`; invalidate only on file modification time change
   - *Why:* Currently recreates all images every second (1000ms interval) even if files unchanged; massive memory/CPU waste
   - *Impact:* **HIGH** — Major performance leak

3. **Debounce `<Configure>` event to avoid redundant renders**
   - *What:* Add debounce timer; only call `update_ui()` after resize stops for 300ms
   - *Why:* `<Configure>` fires on every pixel change; currently triggers full re-render constantly
   - *Impact:* **HIGH** — Reduces unnecessary work by 10x+

4. **Use file system watcher instead of polling loop**
   - *What:* Replace `after(IMAGE_RELOAD_INTERVAL_MS, refresh_images)` with `watchdog` library
   - *Why:* Current polling every 1 second is inefficient; misses rapidly-created files; wastes CPU on idle
   - *Impact:* **MEDIUM** — Elegant and efficient

5. **Implement lazy loading / virtualization for large image sets**
   - *What:* Only render visible images; stream off-screen images
   - *Why:* Current approach loads all images at once; fails with 1000+ images in temp folder
   - *Impact:* **MEDIUM** — Scales to production volumes

### 🛡️ **ERROR HANDLING & ROBUSTNESS** (High Impact)

1. **Add comprehensive error handling**
    - *What:* Catch and gracefully handle: missing folder, corrupted images, permission denied, disk full, invalid paths
    - *Why:* Only catches `PermissionError` in `AppBody`; other failures silently crash. `execute_engine()` returns errors but no handling.
    - *Impact:* **HIGH** — App crashes or hangs unexpectedly

2. **Validate config on startup**
    - *What:* Check `IMAGE_TEMP_FOLDER` exists, `ENGINE_EXE_PATH` exists, `IMAGE_RELOAD_INTERVAL_MS > 0`
    - *Why:* Silent failures if config is malformed or paths are invalid
    - *Impact:* **MEDIUM** — Robustness

3. **Add error UI feedback**
    - *What:* Display errors in `AppHeader.info_label` and modal dialogs, not just print statements
    - *Why:* Users don't see errors; they just see app "hang" or "fail silently"
    - *Impact:* **MEDIUM** — User experience and debuggability

4. **Handle engine subprocess failures gracefully**
    - *What:* Check `proc.returncode` in `execute_engine()`; distinguish stderr from crash
    - *Why:* Currently treats any stderr output as success/failure; could mask real crashes
    - *Impact:* **MEDIUM** — Reliability

### 🧪 **TESTABILITY & MAINTAINABILITY** (Medium Impact)

1. **Remove unused imports**
    - *What:* Delete `Iterable` from `appBody.py`; delete `Optional`, `Popen` from other files
    - *Why:* Linter errors; poor code hygiene
    - *Impact:* **LOW** — Code quality

2. **Add logging instead of print statements**
    - *What:* Replace all `print()` with `logging` module; configure file logging
    - *Why:* Print is lost/invisible in production; no severity levels, timestamps, or log rotation
    - *Impact:* **MEDIUM** — Debuggability and production monitoring

3. **Extract magic numbers to config**
    - *What:* Move `PADDING=8`, `MAX_COLS=4`, `IMAGE_WIDTH`, debounce delay, `MAX_LEN=128` to `config.py`
    - *Why:* Hard-coded values are unmaintainable; users can't customize without code changes
    - *Impact:* **MEDIUM** — Maintainability and flexibility

4. **Add docstrings and type hints**
    - *What:* Document `AppBody.get_images()`, `refresh_images()`, `update_ui()`, `AppHeader.click_run()`
    - *Why:* No documentation of behavior, parameters, return values; future maintainers confused
    - *Impact:* **MEDIUM** — Developer experience

5. **Create unit tests for image discovery logic**
    - *What:* Mock `Path.rglob()`, test layout calculations independently
    - *Why:* Currently untestable; logic tightly coupled to Tkinter
    - *Impact:* **MEDIUM** — Confidence in changes

### 📦 **DATA FLOW & STATE** (Medium Impact)

1. **Fix `AppHeader.click_open_image()` bug**
    - *What:* Use `path.name` (should be full path); store as `Path` object, not string
    - *Why:* `filedialog.askopenfile()` returns file object; `.name` is only filename, not full path. Engine needs full path.
    - *Impact:* **HIGH** — Feature is broken

2. **Make `AppState` immutable and use proper update mechanism**
    - *What:* Use dataclass with `frozen=True`; create new instances instead of mutating
    - *Why:* Current mutation (`self._app_state.image_path = ...`) is error-prone; no way to subscribe to changes
    - *Impact:* **MEDIUM** — Prevents bugs, enables reactivity

3. **Pass state down, not sideways**
    - *What:* `App` should hold state; pass it to `AppHeader` and `AppBody` as props; use callbacks to update
    - *Why:* Currently `AppHeader` mutates its own state and updates labels; `AppBody` never sees it
    - *Impact:* **MEDIUM** — Unidirectional data flow

### 🔄 **ASYNC & THREADING** (Medium Impact)

1. **Standardize threading pattern**
    - *What:* Use `threading.Thread` consistently (as in `click_run()`); apply same pattern to image loading
    - *Why:* `execute_engine()` uses `asyncio` but called from thread; potential race conditions
    - *Impact:* **MEDIUM** — Prevents deadlocks and race conditions

2. **Use thread-safe queue for image updates**
    - *What:* Post image updates to Tkinter's event loop via `.after()` from worker threads
    - *Why:* Currently `Image.open()` in main thread; later will be in background thread without proper synchronization
    - *Impact:* **MEDIUM** — Prevents crashes and data corruption

### 📐 **CODE ORGANIZATION** (Low-Medium Impact)

1. **Move UI constants to separate file**
    - *What:* Create `ui_constants.py` with `PADDING`, `MAX_COLS`, `IMAGE_WIDTH`, debounce values
    - *Why:* `config.py` is for app configuration; UI theming should be separate
    - *Impact:* **LOW** — Organization

2. **Add README documenting architecture and data flow**
    - *What:* ASCII diagram: `App → (AppHeader, AppBody)`, event flow, state updates
    - *Why:* Current architecture is implicit; new developers can't understand it
    - *Impact:* **LOW** — Onboarding

---

## 🎯 **Implementation Priority**

### **Phase 1: Critical Bugs & Blocking Issues (Week 1)**

- Fix `AppHeader.click_open_image()` bug (#19)
- Remove unused imports (#14)
- Add basic error handling in `AppBody.update_ui()` (#10)

### **Phase 2: Foundation & Scalability (Week 2-3)**

- Extract `ImageService` (#1)
- Implement image caching (#6)
- Add file watcher + debouncing (#7, #8)
- Move image loading to thread (#5)

### **Phase 3: Architecture & State (Week 3-4)**

- Implement pub/sub state manager (#2)
- Break coupling via event system (#3)
- Unidirectional data flow (#21)
- Immutable state (#20)

### **Phase 4: Polish & Testing (Week 4+)**

- Logging and error UI (#12, #15)
- Extract config values (#16)
- Unit tests (#18)
- Documentation (#25)

---

**Impact Summary:** Implementing Phases 1-2 will make the app 5-10x faster, reliable, and testable. Phase 3 enables scaling to 10+ image batches per second.
