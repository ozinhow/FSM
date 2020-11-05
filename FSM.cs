using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FSM : MonoBehaviour
{
    [SerializeField] private _Time _time;
    [SerializeField] private LayerMask _layer;

    private float _dist_ataque = 1f;
    private float _dist_raio = 1.5f;
    private float _dist_parada = 1.5f;
    public GameManager gameMan;

    private Vector3 _destino;
    private Quaternion _rotacao;
    private Vector3 _direcao;
    private FSM _alvo;
    private estado _estado_atual;

    private void Start()
    {
        gameMan = (GameManager)FindObjectOfType(typeof(GameManager));
    }

    private void Update()
    {
        if (_estado_atual != estado.normal && _time == _Time.cacador)
        {
            Debug.Log(_estado_atual);
        }

        switch (_estado_atual)
        {
            case estado.normal:
                {
                    if (precisa_destino())
                    {
                        destino();
                    }

                    transform.rotation = _rotacao;
                    
                    transform.Translate(Vector3.forward * Time.deltaTime * 5f);

                    var rayColor = caminho_bloqueado();

                    while (caminho_bloqueado())
                    {
                        destino();
                    }

                    var alvo_para_fugir = verificar_ataque();
                    if (alvo_para_fugir != null && _time == _Time.caca)
                    {
                        destino();
                    }

                    var alvo_para_atacar = verificar_ataque();
                    if (alvo_para_atacar != null && _time == _Time.cacador)
                    {
                        _alvo = alvo_para_atacar.GetComponent<FSM>();
                        _estado_atual = estado.perseguir;
                    }
                    else if (alvo_para_atacar != null && _time == _Time.caca)
                    {
                      
                        _estado_atual = estado.fugir;
                    }

                    break;
                }
            case estado.perseguir:
                {
                    if (_alvo == null)
                    {
                        _estado_atual = estado.normal;
                        return;
                    }

                    transform.LookAt(_alvo.transform);
                    transform.Translate(Vector3.forward * Time.deltaTime * 7f);

                    var rayColor = caminho_bloqueado();

                    while (caminho_bloqueado())
                    {
                        destino();
                    }

                    if (Vector3.Distance(transform.position, _alvo.transform.position) < _dist_ataque)
                    {
                        _estado_atual = estado.atacar;
                    }
                    break;
                }
            case estado.atacar:
                {
                    if (_alvo != null)
                    {
                        Destroy(_alvo.gameObject);
                        gameMan.qtdBicho--;

                    }

                    _estado_atual = estado.normal;
                    break;
                }
            case estado.fugir:
                {
                    var _ver_cacador = verificar_ataque();
                    if (_ver_cacador != null)
                    {
                        destino();
                    }

                    transform.rotation = _rotacao;

                    transform.Translate(Vector3.forward * Time.deltaTime * 5.3f);

                    var rayColor = caminho_bloqueado();

                    while (caminho_bloqueado())
                    {
                        destino();
                    }

                    break;
                }
        }
    }

    private bool caminho_bloqueado()
    {
        Ray ray = new Ray(transform.position, _direcao);
        var hitSomething = Physics.RaycastAll(ray, _dist_raio, _layer);
        return hitSomething.Any();
    }

    private void destino()
    {
        Vector3 testPosition = (transform.position + (transform.forward * 4f)) +
                               new Vector3(UnityEngine.Random.Range(-4.5f, 4.5f), 0f,
                                   UnityEngine.Random.Range(-4.5f, 4.5f));

        _destino = new Vector3(testPosition.x, 1f, testPosition.z);

        _direcao = Vector3.Normalize(_destino - transform.position);
        _direcao = new Vector3(_direcao.x, 0f, _direcao.z);
        _rotacao = Quaternion.LookRotation(_direcao);
    }

    private bool precisa_destino()
    {
        if (_destino == Vector3.zero)
            return true;

        var distance = Vector3.Distance(transform.position, _destino);
        if (distance <= _dist_parada)
        {
            return true;
        }

        return false;
    }

    Quaternion anguloMira = Quaternion.AngleAxis(-60, Vector3.up);
    Quaternion espacoMira = Quaternion.AngleAxis(5, Vector3.up);

    private Transform verificar_ataque()
    {
        float distMira = 3.5f;

        RaycastHit hit;
        var angle = transform.rotation * anguloMira;
        var direction = angle * Vector3.forward;
        var pos = transform.position;
        for (var i = 0; i < 24; i++)
        {
            if (Physics.Raycast(pos, direction, out hit, distMira))
            {
                var agente = hit.collider.GetComponent<FSM>();
                if (agente != null && agente._time != gameObject.GetComponent<FSM>()._time)
                {
                    return agente.transform;
                }
            }
            direction = espacoMira * direction;
        }

        return null;
    }
}

public enum _Time
{
    cacador,
    caca
}

public enum estado
{
    normal,
    perseguir,
    atacar, 
    fugir
}

