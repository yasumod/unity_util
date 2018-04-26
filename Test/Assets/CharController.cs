
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Animator))]
public class CharController :  BaseCharController
{

    public Transform camera;
    public Transform minimapCamera;

    /// <summary>
    /// Update
    /// </summary>
    protected  void Update()
    {
        float X_Rotation = Input.GetAxis("Mouse X") * kando;
        float Y_Rotation = Input.GetAxis("Mouse Y") * kando * -1;
        transform.Rotate(0, X_Rotation, 0);
        camera.transform.Rotate(Y_Rotation, 0, 0);
        float curSpeed = speed;
    
        rigibody.velocity = Vector3.zero;
        InputUtil.MoveCommand command = InputUtil.GetMoveCommand();

        // モーション更新
        UpdateMotion(command.xy.x, command.xy.y, command.IsDash());

        // 移動コマンドが指定されてなければ何もしない
        if (!command.IsMove())
        {
            return;
        }

        // ダッシュ中なら速度を倍に変更
        if (command.IsDash())
        {
            curSpeed *= 2;
        }

        // 物理移動
        rigibody.velocity = (Quaternion.Euler(0f, command.rotation, 0f) * transform.forward) * curSpeed * Time.deltaTime;

        // ミニマップを追従させる
        minimapCamera.position = new Vector3(transform.position.x, minimapCamera.position.y,transform.position.z);
    }

    private void UpdateMotion(float x, float y, bool isDash)
    {
        //モーション判定用のパラメータ   
        animator.SetBool("IsIdle", x == 0 && y == 0);
        animator.SetBool("IsDash", isDash);
        animator.SetFloat("X", x);
        animator.SetFloat("Y", y);
    }

}