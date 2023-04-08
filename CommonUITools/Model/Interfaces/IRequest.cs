﻿namespace CommonUITools.Model.Interfaces;

public interface IRequest {
    public event EventHandler Requested;
}

public interface IRequest<T> : IRequest {
    public new event EventHandler<T> Requested;
}