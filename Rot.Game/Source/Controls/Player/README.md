There's a hack to avoid stack overflow: clearing the buffer of the virtual input. Then the `PlayerControl` doesn't make any action when entering it again in the same frame.
