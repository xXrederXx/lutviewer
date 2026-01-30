from typing import Literal
from dataclasses import dataclass


@dataclass
class AppState:
    luts_folder: str
    image_path: str
    image_width: int


APP_NAME: str = "Lut Viewer"
APP_SIZE: str = "1200x800"
APP_THEME: Literal["blue", "green", "dark-blue"] = "dark-blue"
APP_APPERANCE: Literal["dark", "light", "system"] = "dark"
HEADER_FONT = ("sans serif", 28, "bold")
