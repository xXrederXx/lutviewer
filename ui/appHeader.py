from typing import Any
import customtkinter as ctk


class AppHeader(ctk.CTkFrame):
    def __init__(self, master: Any):
        super().__init__(master, height=80, width=9999)
