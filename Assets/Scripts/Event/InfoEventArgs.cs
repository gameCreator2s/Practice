using UnityEngine;
using System;
using System.Collections;

public class InfoEventArgs<T> : EventArgs {

    public T info;
    public InfoEventArgs() {
        info = default(T);
    }
    public InfoEventArgs(T info) {
        this.info = info;
    }
}
