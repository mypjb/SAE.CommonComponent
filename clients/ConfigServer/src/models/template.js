import { useCallback, useEffect, useState } from "react";
import request from "@/pages/template/service";

export default () => {
    const [state, setState] = useState([]);
    console.log(request);
    const load = useCallback(() => {
        request.list()
            .then((data) => {
                setState(data);
            }).catch((ex) => {
                console.error(ex);
            });
    }, []);
    useEffect(() => {
        load();
    }, []);
    
    return {
        state,
        load
    }
}