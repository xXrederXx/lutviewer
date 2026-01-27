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
        
        
        self.image_btn = ctk.CTkButton(self, text="Open Image")
        self.image_btn.grid(row=0, column=1, padx=12)
        
        self.lut_btn = ctk.CTkButton(self, text="Open LUTs")
        self.lut_btn.grid(row=0, column=2, padx=12)
        
        self.preview_size_dropdown = ctk.CTkOptionMenu(self, values=["Auto", "Full", "Half", "Quater", "Eigth"])
        self.preview_size_dropdown.grid(row=0, column=3, padx=12)
