from typing import Any
import customtkinter as ctk
from config import APP_NAME, HEADER_FONT


class AppHeader(ctk.CTkFrame):
    def __init__(self, master: Any):
        super().__init__(master)

        self.grid_rowconfigure(0, weight=1)
        self.grid_columnconfigure(0, weight=1)

        self.title = ctk.CTkLabel(self, text=APP_NAME, font=HEADER_FONT, height=60)
        self.title.grid(row=0, column=0, sticky="w", padx=12)
