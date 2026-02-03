from typing import Any, Optional
import customtkinter as ctk
from config import APP_NAME, HEADER_FONT, AppState, IMAGE_TEMP_FOLDER
from engineConnector import EngineArgs, execute_engine
from pathlib import Path
import asyncio
import threading


class AppHeader(ctk.CTkFrame):
    def __init__(self, master: Any):
        super().__init__(master)

        self._app_state: AppState = AppState("", "", -1)

        self.grid_rowconfigure((0, 1), weight=1)
        self.grid_columnconfigure(0, weight=1)

        self.title = ctk.CTkLabel(self, text=APP_NAME, font=HEADER_FONT, height=60)
        self.title.grid(row=0, column=0, sticky="w", padx=12)

        self.run_btn = ctk.CTkButton(self, text="Run", command=self.click_run)
        self.run_btn.grid(row=0, column=1, padx=12)

        self.image_btn = ctk.CTkButton(
            self, text="Open Image", command=self.click_open_image
        )
        self.image_btn.grid(row=0, column=2, padx=12)

        self.lut_btn = ctk.CTkButton(
            self, text="Open LUTs", command=self.click_open_luts
        )
        self.lut_btn.grid(row=0, column=3, padx=12)

        self.preview_size_entry = ctk.CTkEntry(self, placeholder_text="Render Width")
        self.preview_size_entry.grid(row=0, column=4, padx=12)

        self.err_label = ctk.CTkLabel(self, text="")
        self.err_label.grid(row=0, column=0, padx=12)

    def click_run(self):
        args = EngineArgs(
            Path(self._app_state.image_path),
            IMAGE_TEMP_FOLDER,
            self._app_state.image_width,
            Path(self._app_state.luts_folder),
        )
        threading.Thread(
            target=self._run_engine_thread, args=(args,), daemon=True
        ).start()

    def click_open_image(self):
        path = ctk.filedialog.askopenfile()
        if path is not None:
            self._app_state.image_path = path.name

    def _run_engine_thread(self, args: EngineArgs):
        stdout, stderr = asyncio.run(execute_engine(args))
        self.after(0, self._on_engine_done, stdout, stderr)

    def _on_engine_done(self, stdout: str, stderr: str):
        if stderr:
            self.err_label.configure(text=stderr)
        else:
            self.err_label.configure(text="Done!")

    def click_open_luts(self):
        self._app_state.luts_folder = ctk.filedialog.askdirectory()

    def get_app_state(self):
        size: int = -1

        try:
            size = int(self.preview_size_entry.cget("text"))
        except:
            size = -1

        self._app_state.image_width = size
        return AppState(self._app_state.luts_folder, self._app_state.image_path, size)
