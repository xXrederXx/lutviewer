import os
from typing import Literal
from dataclasses import dataclass
import tempfile
from pathlib import Path


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


IMAGE_TEMP_FOLDER = Path(tempfile.gettempdir()) / "LutViewer"

if not os.path.exists(IMAGE_TEMP_FOLDER):
    os.makedirs(IMAGE_TEMP_FOLDER)

IMAGE_RELOAD_INTERVAL_MS = 1000
IMAGE_PATH_PATTERN = "*.png"


ENGINE_EXE_PATH = Path(r"C:\Users\Thierry\DEV\cross\lutviewer\engine\bin\Release\net10.0\engine.exe")

print("LOADED APP CONFIG:")
print(f"\t-{APP_NAME}")
print(f"\t-{APP_SIZE}")
print(f"\t-{APP_THEME}")
print(f"\t-{APP_APPERANCE}")
print(f"\t-{HEADER_FONT}")
print(f"\t-{IMAGE_TEMP_FOLDER}")
