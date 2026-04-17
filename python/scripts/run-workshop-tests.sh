#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
REPO_ROOT="$(cd "${SCRIPT_DIR}/.." && pwd)"
cd "${REPO_ROOT}"

if [[ "${1:-}" == "--require-python-parity" ]]; then
  REQUIRE_PYTHON_PARITY=true dotnet test tests/WorkshopParity.Tests/WorkshopParity.Tests.csproj -v minimal
else
  dotnet test tests/WorkshopParity.Tests/WorkshopParity.Tests.csproj -v minimal
fi
