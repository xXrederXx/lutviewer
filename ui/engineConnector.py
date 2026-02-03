from dataclasses import dataclass
from pathlib import Path
from subprocess import Popen, run, PIPE
from config import ENGINE_EXE_PATH
from typing import Tuple
import asyncio


@dataclass
class EngineArgs:
    image: Path
    out: Path
    size: int
    luts: Path


async def execute_engine(args: EngineArgs) -> Tuple[str, str]:
    proc = await asyncio.create_subprocess_exec(
        str(ENGINE_EXE_PATH.resolve()),
        "-i",
        str(args.image.resolve()),
        "-o",
        str(args.out.resolve()),
        "-s",
        str(args.size),
        "-l",
        str(args.luts.resolve()),
        stdout=PIPE,
        stderr=PIPE,
    )

    stdout, stderr = await proc.communicate()
    stdout = stdout.decode()
    stderr = stderr.decode()

    print(
        f'-i "{args.image.resolve()}"',
        f'-o "{args.out.resolve()}"',
        f"-s {args.size}",
        f'-l "{args.luts.resolve()}',
    )

    print("STDOUT: ", stdout)
    print("STDERR: ", stderr)

    return (stdout, stderr)
