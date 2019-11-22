#!/usr/bin/env python3
import sys
import time
import json


def factorial(n):
    if n == 1:
        return n

    return n * factorial(n-1)


if __name__ == "__main__":
    input = int(sys.argv[1])
    result = factorial(input)
    print(json.dumps({
        'input': input,
        'result': result
    }))
    time.sleep(60)
    sys.exit(0)
