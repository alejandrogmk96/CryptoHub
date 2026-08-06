# Developer Log - Day 05 (Git Fundamentals)

## Objetivo

Hoy fortalecimos los fundamentos de Git antes de continuar con
CryptoHub.

## Temas aprendidos

### bin y obj

-   Son carpetas generadas automáticamente por .NET.
-   No deben formar parte del repositorio.

### .gitignore

Inicialmente se escribió:

``` gitignore
/obj
/bin
```

Después se comprendió por qué no funcionaba.

La configuración correcta quedó:

``` gitignore
obj/
bin/
```

Esto ignora cualquier carpeta llamada `obj` o `bin` dentro del
repositorio.

### ¿Por qué no bastó con .gitignore?

Se comprendió que `.gitignore` **no deja de seguir archivos que Git ya
conoce**.

Fue necesario ejecutar:

``` bash
git rm --cached -r src/CryptoHub.Api/bin
git rm --cached -r src/CryptoHub.Api/obj
```

Aprendizaje: - `git rm` elimina del repositorio y del disco. -
`git rm --cached` elimina únicamente del índice de Git.

## Flujo de Git

``` bash
git status
git add .
git status
git commit -m "..."
git push
```

El segundo `git status` sirve como revisión final.

## Conventional Commits

-   feat → Nueva funcionalidad.
-   fix → Corrección de errores.
-   refactor → Mejora interna sin cambiar comportamiento.
-   chore → Mantenimiento del proyecto.

El commit correcto para esta sesión fue:

``` text
chore: add .gitignore for build artifacts K
```

## Estado final

``` text
On branch main
Your branch is up to date with 'origin/main'.

nothing to commit, working tree clean
```

## Próxima sesión

-   Modelo `Price`.
-   Interfaz `IExchange`.
-   Implementación de `BingXExchange`.
-   Introducción a Dependency Injection.
