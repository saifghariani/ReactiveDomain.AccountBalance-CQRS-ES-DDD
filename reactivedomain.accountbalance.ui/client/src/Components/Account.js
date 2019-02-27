import React  from 'react';

const Account = (props)=>{
    return (
        <tr >
                <td>{props.Id}</td>
                <td>{props.HolderName}</td>
                <td style={{backgroundColor : props.State==="Active"? '#00dc00':'red', width : '20px', borderRadius : '50%'}}></td>
            </tr>
    );
}
export default Account;