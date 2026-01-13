using Godot;
using System;

public partial class CharacterBody2d : CharacterBody2D
{
	[Export] public float VelocidadeAgilidade = 100.0f;
	
	private AnimatedSprite2D _animador;
	private string _ultimaDirecao = "baixo"; // Guarda para onde ele olhou por último

	public override void _Ready()
	{
		// Pegamos a referência do nó de animação
		_animador = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
	}

	public override void _PhysicsProcess(double delta)
	{
		// 1. Captura o input
		Vector2 entradaUsuario = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

		// 2. Aplica o movimento
		if (entradaUsuario != Vector2.Zero)
		{
			Velocity = entradaUsuario * VelocidadeAgilidade;
			ProcessarAnimacao(entradaUsuario, true);
		}
		else
		{
			// Se não há input, o personagem para gradualmente
			Velocity = Velocity.MoveToward(Vector2.Zero, 1000.0f * (float)delta);
			ProcessarAnimacao(Vector2.Zero, false);
		}

		MoveAndSlide();
	}

	private void ProcessarAnimacao(Vector2 direcao, bool estaMovendo)
	{
		// Definimos o prefixo: 'andando' ou 'parado'
		string acao = estaMovendo ? "andando" : "parado";

		if (estaMovendo)
		{
			// LÓGICA DE PRIORIDADE:
			// Comparamos o valor absoluto (sem sinal) de X e Y.
			// Se |X| for maior que |Y|, o movimento é mais horizontal que vertical.
			if (Mathf.Abs(direcao.X) > Mathf.Abs(direcao.Y))
			{
				_ultimaDirecao = "lado";
				// Inverte o desenho se for para a esquerda (X negativo)
				_animador.FlipH = direcao.X < 0;
			}
			else
			{
				// Se |Y| for maior, o movimento é vertical.
				_ultimaDirecao = direcao.Y > 0 ? "baixo" : "cima";
				_animador.FlipH = false; // Reseta o espelhamento
			}
		}

		// Monta o nome final: ex "andando_baixo" ou "parado_lado"
		string nomeAnimacao = $"{acao}_{_ultimaDirecao}";
		
		// Dá o play apenas se a animação não for a que já está tocando
		if (_animador.Animation != nomeAnimacao)
		{
			_animador.Play(nomeAnimacao);
		}
	}
}
