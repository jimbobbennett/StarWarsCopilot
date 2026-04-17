# Danny Decision: Identical Workshop Parity Contract

## Context

We are porting a C# workshop to Python. "Identical" must be defined as **instructional, behavioral, and architectural parity**, not syntax parity.

Primary C# source of truth:
1. Lesson READMEs (`1-8/*/README.md`) for teaching sequence and expected outcomes
2. Lesson `after` code snapshots (`1-8/*/after/**`) for concrete implementation behavior
3. Root `README.md` for repo-level structure and promises

If these conflict, priority is: **after code > lesson README > root README**.

## Decision

The Python migration will follow a strict parity contract with an approval gate for every non-trivial porting decision.

### What "identical workshop" means

1. **Same learning path:** 8 lessons, same order, same progression of concepts.
2. **Same exercise intent:** each exercise teaches the same capability and trade-offs.
3. **Same runtime behaviors:** same user-visible chat/tool/agent outcomes for equivalent prompts and data.
4. **Same integration surfaces:** same external systems (AOAI, Tavily, MCP, Azure Tables, image generation, agents).
5. **Same tool/agent semantics:** equivalent tool names, parameter intent, and orchestration flow.
6. **Same constraints and caveats:** equivalent safety, retries, and known limitations documented to learners.
7. **Equivalent observability:** logging/tracing teaches the same "what happened" story.
8. **Equivalent artifacts:** per-lesson runnable "after" solution and lesson guide remain paired.

### Step-by-step parity criteria (lesson gates)

1. **Lesson 1 – Chat basics:** prompt/response loop parity with explicit model config loading.
2. **Lesson 2 – History & roles:** persisted message history parity, including system prompt behavior.
3. **Lesson 3 – Model choice:** provider swap parity (AOAI vs AI Inference vs local model) behind stable chat abstraction.
4. **Lesson 4 – Tool calling:** Wookiepedia tool parity (schema intent, invocation flow, response handling).
5. **Lesson 5 – MCP:** MCP server/client parity over stdio with dynamic tool discovery.
6. **Lesson 6 – RAG:** purchase lookup tool parity across order/customer/character filters and result shaping.
7. **Lesson 7 – Multimodal:** image tool parity, including content-policy failure guidance and retry instruction pattern.
8. **Lesson 8 – Agents:** single-agent and supervisor multi-agent orchestration parity, including tool-mediated delegation.

## Approved migration architecture

1. Keep top-level lesson folder numbering (`1-8`) and each lesson's `README.md` as migration anchors.
2. Keep per-lesson `after` runnable solution pattern.
3. Preserve C# conceptual boundaries in Python:
   - copilot app
   - MCP server
   - tools/options modules
   - agents modules
4. Permit language-native implementation differences only where required by SDK/runtime differences, while preserving behavior and learning outcomes.

## Porting approval policy (mandatory)

Every porting PR/change must include:
1. **C# references** (file + relevant lines/sections)
2. **Python counterpart** (file + behavior mapping)
3. **Parity classification**:
   - **Direct parity** (same behavior, different syntax)
   - **Equivalent substitution** (SDK/runtime difference, same outcome)
   - **Intentional divergence** (rare; requires explicit approval)
4. **Behavior evidence** showing expected output/flow parity for that lesson gate

Approval rules:
1. Auto-approve only **Direct parity** and **Equivalent substitution** with complete evidence.
2. **Intentional divergence** requires explicit Danny sign-off with documented rationale and blast radius.
3. Any change that alters lesson sequence, tool/agent semantics, or learner-visible outcomes is rejected unless pre-approved.

## Current approval status

Approved now:
1. This parity contract and gate model
2. The eight lesson gates listed above
3. Source-of-truth precedence (after code > lesson README > root README)

Pending:
1. File-by-file Python implementation approvals as Rusty/Linus/Livingston produce ports
