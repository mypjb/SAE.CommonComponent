import { useCallback, useState } from "react"
import request from "../pages/template/service";

export default () => {
    const [state, setState] = useState([]);
    const load = useCallback(() => {
        request.list()
            .then((data) => {
                setState(data);
            }).catch((ex) => {
                console.error(ex);
            });
    }, []);
    console.log(22);
    return {
        state,
        load
    }
}