﻿using System.ComponentModel;
using System.Windows.Input;

namespace Endure.Controls;

public class ExpandedChangedEventArgs : EventArgs
{
	public bool IsExpanded { get; }

	public ExpandedChangedEventArgs(bool isExpanded) => IsExpanded = isExpanded;
}

[ContentProperty(nameof(Content))]
public class Expander : ContentView, IExpander
{
	/// <summary>
	/// Backing BindableProperty for the <see cref="Header"/> property.
	/// </summary>
	public static readonly BindableProperty HeaderProperty
		= BindableProperty.Create(nameof(Header), typeof(IView), typeof(Expander), propertyChanged: OnHeaderPropertyChanged);

	/// <summary>
	/// Backing BindableProperty for the <see cref="Content"/> property.
	/// </summary>
	public static new readonly BindableProperty ContentProperty
		= BindableProperty.Create(nameof(Content), typeof(IView), typeof(Expander), propertyChanged: OnContentPropertyChanged);

	/// <summary>
	/// Backing BindableProperty for the <see cref="IsExpanded"/> property.
	/// </summary>
	public static readonly BindableProperty IsExpandedProperty
		= BindableProperty.Create(nameof(IsExpanded), typeof(bool), typeof(Expander), false, BindingMode.TwoWay, propertyChanged: OnIsExpandedPropertyChanged);

	/// <summary>
	/// Backing BindableProperty for the <see cref="Direction"/> property.
	/// </summary>
	public static readonly BindableProperty DirectionProperty
		= BindableProperty.Create(nameof(Direction), typeof(ExpandDirection), typeof(Expander), ExpandDirection.Down, propertyChanged: OnDirectionPropertyChanged);

	/// <summary>
	/// Backing BindableProperty for the <see cref="CommandParameter"/> property.
	/// </summary>
	public static readonly BindableProperty CommandParameterProperty
		= BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(Expander));

	/// <summary>
	/// Backing BindableProperty for the <see cref="Command"/> property.
	/// </summary>
	public static readonly BindableProperty CommandProperty
		= BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(Expander));

	public event EventHandler<ExpandedChangedEventArgs> ExpandedChanged
	{
		add => m_tappedEventManager.AddEventHandler(value);
		remove => m_tappedEventManager.RemoveEventHandler(value);
	}

	internal TapGestureRecognizer HeaderTapGestureRecognizer { get; } = new();

	/// <summary>
	/// The Action that fires when <see cref="Header"/> is tapped.
	/// By default, this <see cref="Action"/> runs <see cref="ResizeExpanderInItemsView(TappedEventArgs)"/>.
	/// </summary>
	public Action<TappedEventArgs>? HandleHeaderTapped { get; set; }

	public IView? Header
	{
		get => (IView?)GetValue(HeaderProperty);
		set => SetValue(HeaderProperty, value);
	}

	public new IView? Content
	{
		get => (IView?)GetValue(Expander.ContentProperty);
		set => SetValue(Expander.ContentProperty, value);
	}

	public bool IsExpanded
	{
		get => (bool)GetValue(IsExpandedProperty);
		set => SetValue(IsExpandedProperty, value);
	}

	public ExpandDirection Direction
	{
		get => (ExpandDirection)GetValue(DirectionProperty);
		set
		{
			if (!Enum.IsDefined(typeof(ExpandDirection), value))
			{
				throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ExpandDirection));
			}

			SetValue(DirectionProperty, value);
		}
	}

	/// <summary>
	/// Command parameter passed to the <see cref="Command"/>
	/// </summary>
	public object? CommandParameter
	{
		get => GetValue(CommandParameterProperty);
		set => SetValue(CommandParameterProperty, value);
	}

	/// <summary>
	/// Command to execute when <see cref="IsExpanded"/> changed.
	/// </summary>
	public ICommand? Command
	{
		get => (ICommand?)GetValue(CommandProperty);
		set => SetValue(CommandProperty, value);
	}

	private Grid ContentGrid => (Grid)base.Content;

	private readonly WeakEventManager m_tappedEventManager = new();

	public Expander()
	{
		HandleHeaderTapped = ResizeExpanderInItemsView;
		HeaderTapGestureRecognizer.Tapped += OnHeaderTapGestureRecognizerTapped;

		base.Content = new Grid
		{
			RowDefinitions =
			{
				new RowDefinition(GridLength.Auto),
				new RowDefinition(GridLength.Auto)
			}
		};
	}

	private static void OnContentPropertyChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var expander = (Expander)bindable;
		if (newValue is View view)
		{
			view.SetBinding(IsVisibleProperty, new Binding(nameof(IsExpanded), source: bindable));

			expander.ContentGrid.Remove((IView)oldValue);
			expander.ContentGrid.Add(view);
			expander.ContentGrid.SetRow(view, expander.Direction switch
			{
				ExpandDirection.Down => 1,
				ExpandDirection.Up => 0,
				_ => throw new NotSupportedException($"{nameof(ExpandDirection)} {expander.Direction} is not yet supported")
			});
		}
	}

	private static void OnHeaderPropertyChanged(BindableObject bindable, object oldValue, object newValue)
	{
		var expander = (Expander)bindable;
		if (newValue is View view)
		{
			expander.SetHeaderGestures(view);

			expander.ContentGrid.Remove((IView)oldValue);
			expander.ContentGrid.Add(view);

			expander.ContentGrid.SetRow(view, expander.Direction switch
			{
				ExpandDirection.Down => 0,
				ExpandDirection.Up => 1,
				_ => throw new NotSupportedException($"{nameof(ExpandDirection)} {expander.Direction} is not yet supported")
			});
		}
	}

	private static void OnIsExpandedPropertyChanged(BindableObject bindable, object oldValue, object newValue) =>
		((IExpander)bindable).ExpandedChanged(((IExpander)bindable).IsExpanded);

	private static void OnDirectionPropertyChanged(BindableObject bindable, object oldValue, object newValue) =>
		((Expander)bindable).HandleDirectionChanged((ExpandDirection)newValue);

	private void HandleDirectionChanged(ExpandDirection expandDirection)
	{
		if (Header is null || Content is null) return;

		switch (expandDirection)
		{
			case ExpandDirection.Down:
				ContentGrid.SetRow(Header, 0);
				ContentGrid.SetRow(Content, 1);
				break;
			case ExpandDirection.Up:
				ContentGrid.SetRow(Header, 1);
				ContentGrid.SetRow(Content, 0);
				break;
			default:
				throw new NotSupportedException($"{nameof(ExpandDirection)} {expandDirection} is not yet supported");
		}
	}

	private void SetHeaderGestures(in IView header)
	{
		var headerView = (View)header;
		headerView.GestureRecognizers.Remove(HeaderTapGestureRecognizer);
		headerView.GestureRecognizers.Add(HeaderTapGestureRecognizer);
	}

	private void OnHeaderTapGestureRecognizerTapped(object? sender, TappedEventArgs tappedEventArgs)
	{
		IsExpanded = !IsExpanded;
		HandleHeaderTapped?.Invoke(tappedEventArgs);
	}

	private void ResizeExpanderInItemsView(TappedEventArgs tappedEventArgs)
	{
		if (Header is null) return;

		Element element = this;

		while (element is not null)
			element = element.Parent;
	}

	void IExpander.ExpandedChanged(bool isExpanded)
	{
		if (Command?.CanExecute(CommandParameter) is true)
		{
			Command.Execute(CommandParameter);
		}

		m_tappedEventManager.HandleEvent(this, new ExpandedChangedEventArgs(isExpanded), nameof(ExpandedChanged));
	}
}