from typing import Optional
import customtkinter as ctk
from appHeader import AppHeader
from config import APP_SIZE, APP_THEME, APP_APPERANCE, APP_NAME, AppState
from appBody import AppBody


class App(ctk.CTk):
    def __init__(self):
        super().__init__()

        self.geometry(APP_SIZE)
        self.title(APP_NAME)
        ctk.set_default_color_theme(APP_THEME)
        ctk.set_appearance_mode(APP_APPERANCE)

        self.grid_rowconfigure(1, weight=1)
        self.grid_columnconfigure(0, weight=1)

        self.header = AppHeader(self)
        self.header.grid(row=0, column=0, sticky="nsew")
        
        self.body = AppBody(self)
        self.body.grid(row=1, column=0, sticky="nsew")
