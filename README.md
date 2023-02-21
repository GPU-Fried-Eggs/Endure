<div align="center">

# Endure

Production brings more laziness. Laziness?... 

</div>

## Design

Page design and logic:

```mermaid
flowchart TD
    master[A searching page]
    memo[Revision page]
    analyze[Analyze page]
    editor[Memo edit page]
    master --> editor
    editor --> master
    master --> memo --> master
    master --> analyze --> master
    memo --> analyze
```

## Description

The main purpose of this program is to overcome my laziness in memorizing words.
It will provide 
