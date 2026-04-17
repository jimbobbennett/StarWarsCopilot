# Basher Decision: Workshop Test Harness and Parity Gates

## Context

Need executable coverage that protects all 8 workshop steps and supports side-by-side behavior checks between the canonical C# lessons and the Python port as it lands.

## Decision

Implemented a dedicated xUnit suite at `tests/WorkshopParity.Tests` with:

1. Step-by-step unit/integration/regression checks over each lesson's `after` artifacts and key behavior markers.
2. Progressive capability regression assertions (history, tool invocation, MCP, retry flow, agent orchestration).
3. MCP server tool progression checks across steps 5→6→7.
4. Data-loader regression checks for lesson 6 seed content and order/customer logic markers.
5. Side-by-side parity checks that compare Python `after-python` artifacts against C# behavior markers whenever `after-python` exists.
6. Strict parity mode (`REQUIRE_PYTHON_PARITY=true`) to fail if any lesson lacks `after-python`.

Also added `scripts/run-workshop-tests.sh` for standard and strict parity runs, and documented usage in `README.md`.
