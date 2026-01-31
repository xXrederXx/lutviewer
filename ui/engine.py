from dataclasses import dataclass
from pathlib import Path
from subprocess import Popen, run, PIPE
from config import ENGINE_EXE_PATH


@dataclass
class EngineArgs:
    image: Path
    out: Path
    size: int
    luts: Path


def execute_engine(args: EngineArgs):
    list_args = [
        str(ENGINE_EXE_PATH.resolve()),
        "-i",
        str(args.image.resolve()),
        "-o",
        str(args.out.resolve()),
        "-s",
        str(args.size),
        "-l",
        str(args.luts.resolve()),
    ]

    result = run(
        list_args,
        text=True,
        stdout=PIPE,
        stderr=PIPE,
        check=False,
    )

    print(list_args)

    print(result.stdout)
    print(result.stderr)
