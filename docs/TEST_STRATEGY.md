# Test strategy

System under test: bundled browser-only TaskBoard. Scope: functional UI and DOM behaviors in Chrome, headless by default. Twenty-three NUnit cases exercise positive, negative, boundary and state transitions.

Each case receives a fresh browser profile, so task names need no randomization for isolation. The app/server are controlled; tests do not depend on public demo services. POM contains interactions, and tests assert user-visible outcomes.

Known gaps: no independent backend, cross-browser coverage, accessibility audit, performance checks or complete security coverage. The HTML-like title case verifies DOM text rendering only. The demo's simple authentication cannot establish real access-control quality.

CI must pass before recording evidence. Investigate infrastructure/driver failures separately from application assertions. Record failing screenshot, DOM and TRX when available.
