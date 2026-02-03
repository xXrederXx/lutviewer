import customtkinter as ctk
from typing import Any, Iterable, List
from config import IMAGE_RELOAD_INTERVAL_MS, IMAGE_TEMP_FOLDER, IMAGE_PATH_PATTERN
from pathlib import Path
from PIL import Image


class AppBody(ctk.CTkScrollableFrame):
    def __init__(self, master: Any):
        super().__init__(master)

        self.image_paths: List[Path] = []
        self.last_width = -1

        self.after(IMAGE_RELOAD_INTERVAL_MS, self.refresh_images)
        self.bind("<Configure>", lambda e: self.update_ui(), "+")

    def get_images(self):
        files = Path(IMAGE_TEMP_FOLDER).rglob(IMAGE_PATH_PATTERN)
        return list(files)

    def refresh_images(self):
        self.update_ui()
        self.after(IMAGE_RELOAD_INTERVAL_MS, self.refresh_images)

    def update_ui(self):
        images = self.get_images()
        width = self.winfo_width()
        
        if images == self.image_paths and self.last_width == width:
            return

        self.image_paths = images
        self.last_width = width

        for child in self.winfo_children():
            child.destroy()

        row = 0
        col = 0
        PADDING = 8
        MAX_COLS = 4
        IMAGE_WIDTH = width // MAX_COLS - 2 * PADDING

        for path in images:
            item = ctk.CTkFrame(self, fg_color="transparent")
            item.grid(row=row, column=col, padx=PADDING, pady=PADDING, sticky="n")

            # filename label (top)
            name_label = ctk.CTkLabel(
                item,
                text=path.name,
            )
            name_label.pack(pady=(0, 4))

            # load image
            try:
                with Image.open(path) as pil_image:
                    image = ctk.CTkImage(
                        light_image=pil_image,
                        dark_image=pil_image,
                        size=(
                            int(IMAGE_WIDTH),
                            int(pil_image.height / pil_image.width * IMAGE_WIDTH),
                        ),
                    )

                    image_label = ctk.CTkLabel(item, text="", image=image)
                    image_label.pack()
            except PermissionError as ex:
                print("Could not open file, cause: permission error.", ex, path)
                continue

            # grid layout logic
            col += 1
            if col >= MAX_COLS:
                col = 0
                row += 1
