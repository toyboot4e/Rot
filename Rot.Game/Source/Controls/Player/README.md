There's a hack to avoid stack overflow: clearing the buffer of the virtual input. Then the `PlayerControl` doesn't return same action when entered in the same frame again.
