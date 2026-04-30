# Claude Code — точка входу

## Контекст проєкту
Перед початком роботи прочитай:
1. `docs/AI_Context/project-context.md` — архітектура, шари, стек, патерни
2. `docs/AI_Context/patterns.md` — деталі реалізованих патернів
3. `docs/AI_Context/decisions/ADR-*.md` — архітектурні рішення (за темою задачі)

## Правила роботи
- `docs/AI_Settings/copilot-instructions.md` — ToT-версіонування, статуси, режими AI, робота з БД, маркування, git-конвенції
- `docs/AI_Settings/rent-for-students-architect.yaml` — код-стандарти (var заборонено, виключення: `new Type(...)`)

## Логування
- `docs/AI_Log/PROMPT_HISTORY.md` — індекс сесій
- `docs/AI_Log/sessions/*.md` — деталі сесій

## Git-навігація
- Теги позначають великі версії: `git tag -l`
- Перегляд змін між версіями: `git diff vX.Y..vX.Z`
- Пошук комітів за версією: `git log --oneline --all --grep="vX.Y"`

## Мова
- Документація та спілкування: українська
- Код та коментарі в коді: англійська
