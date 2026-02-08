from pathlib import Path
from typing import Dict, List, Tuple

import customtkinter as ctk
from config import IMAGE_PATH_PATTERN, IMAGE_TEMP_FOLDER
from PIL import Image


class ImageService:
    """This class provides functionality to load images and their ui objects"""

    def __init__(self) -> None:
        self.image_folder = IMAGE_TEMP_FOLDER
        self.image_file_pattern = IMAGE_PATH_PATTERN

        self.image_cache: Dict[Path, ctk.CTkImage] = {}

    def get_image_paths(self) -> List[Path]:
        """Searches the image_folder with a recursive pattern and returns the results in a list.

        Returns:
            List[Path]: The list of file paths which match the pattern
        """
        return list(Path(self.image_folder).rglob(self.image_file_pattern))

    def get_ctk_images(self) -> List[Tuple[Path, ctk.CTkImage]]:
        """Searches all images and returns a list of image elements inculuding the path
        It also uses cached data if possible

        Returns:
            List[Tuple[Path, ctk.CTkImage]]: A list containing tuples of paths to
            image files and their ui elements
        """
        current_paths = self.get_image_paths()

        new_images: Dict[Path, ctk.CTkImage] = {}

        for path in current_paths:
            image = self.image_cache.get(path, None)

            if image is None:
                image = self.load_image(path)

            new_images[path] = image

        self.image_cache = new_images

        return list(new_images.items())

    def load_image(self, path: Path) -> ctk.CTkImage:
        """Loads an image and creates a default ctk.CTkImage out of it

        Args:
            path (Path): The path to the image file requiered to load

        Raises:
            ValueError: Raised if the path is invalid
            PermissionError: Raised if the app dosnt have the right permissions

        Returns:
            ctk.CTkImage: The newly loaded image ui element
        """
        if not path.is_file():
            raise ValueError("Path was not a file!", path)

        try:
            with Image.open(path) as pil_image:
                return ctk.CTkImage(
                    light_image=pil_image.copy(),
                    dark_image=pil_image.copy(),
                    size=(
                        pil_image.width,
                        pil_image.height,
                    ),
                )

        except PermissionError as ex:
            raise PermissionError(
                "The file could not be accessed, due to insufficent permissions"
            ) from ex
